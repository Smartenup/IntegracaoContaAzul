using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.ContaAzul.Controllers;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Lib;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using Nop.Plugin.Misc.ContaAzul.Service;
using Nop.Plugin.Misc.ContaAzul.Util.Helper;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul
{
    public class ContaAzulSincronizaClienteTask: ITask
    {

        private IContaAzulCustomerService _contaAzulCustomerService;
        private IContaAzulService _contaAzulService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ILogger _logger;

        public ContaAzulSincronizaClienteTask(IContaAzulCustomerService contaAzulCustomerService,
            ISettingService settingService,
            IContaAzulService contaAzulService,
            ICustomerService customerService,
            IStoreService storeService,
            IWorkContext workContext,
            IAddressAttributeParser addressAttributeParser,
            ILogger logger)
        {
            _contaAzulCustomerService = contaAzulCustomerService;
            _contaAzulService = contaAzulService;
            _customerService = customerService;
            _storeService = storeService;
            _addressAttributeParser = addressAttributeParser;
            _logger = logger;
            _settingService = settingService;

        }

        public void Execute()
        {
            _contaAzulService.RefreshToken();

            var customers = _customerService.GetAllCustomers();
           // var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            CustomerResponse[] GetCustomerResponse = null;
            CustomerResponse CustomerResponse = null;
              var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>();

            var number = string.Empty;
            var complement = string.Empty;
            var cpfCnpj = string.Empty;

            foreach (var item in customers)
            {
                var customer = new CustomerMessage();

                new AddressHelper(_addressAttributeParser, _workContext).GetCustomNumberAndComplement(item.BillingAddress != null ? item.BillingAddress.CustomAttributes : null,
                out number, out complement, out cpfCnpj);

                customer.name = item.BillingAddress != null ? AddressHelper.GetFullName(item.BillingAddress) : null;
                customer.companyName = item.BillingAddress != null ? item.BillingAddress.Company : null;
                customer.email = item.Email;
                customer.personType = "NATURAL";
                customer.stateRegistrationType = "NO_CONTRIBUTOR";
                customer.mobilePhone = item.BillingAddress != null ? item.BillingAddress.PhoneNumber : null;
                customer.address.city.name = item.BillingAddress != null ? item.BillingAddress.City : null;
                customer.address.state.name = item.BillingAddress != null ? item.BillingAddress.StateProvince != null ? item.BillingAddress.StateProvince.Name : null : null;
                 customer.address.zipCode = item.BillingAddress != null ? item.BillingAddress.ZipPostalCode : null;
                customer.address.street = item.BillingAddress != null ? item.BillingAddress.Address1 : null;
                customer.address.complement = complement;
                customer.address.number = number;
                customer.document = cpfCnpj == "" ? null : cpfCnpj;

                try
                {
                    var filtro = "?search=";
                    if (cpfCnpj == string.Empty)
                    {
                         filtro = filtro + item.Email;
                    }
                    else
                    {
                         filtro = filtro + cpfCnpj;
                    }
                    using (var getcustomer = new GetCustomer(ContaAzulMiscSettings.UseSandbox))
                        GetCustomerResponse = getcustomer.CreateAsync(null, ContaAzulMiscSettings.access_token, filtro).ConfigureAwait(false).GetAwaiter().GetResult();
                    //busca por cpf conta azul, se existir, verifica se já foi adicionado na tabela do banco
                    if (GetCustomerResponse.Count() > 0)
                    {
                        var customerTable = _contaAzulCustomerService.GetCustomer(GetCustomerResponse[0]);
                        //caso ele não exista na tabela relacional do banco, insere e atualiza no conta azul
                        if (customerTable == null)
                        {
                            using (var customerCreation = new CustomerCreation(ContaAzulMiscSettings.UseSandbox))
                                CustomerResponse = customerCreation.CreateAsyncUpdate(customer, GetCustomerResponse[0].id.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

                            if (CustomerResponse != null)
                            {
                                var customerContaAzul = new CustomerContaAzul();

                                customerContaAzul.ContaAzulId = CustomerResponse.id;
                                customerContaAzul.CustomerId = item.Id;
                                customerContaAzul.DataCriacao = DateTime.Now;
                                _contaAzulCustomerService.InsertCustomer(customerContaAzul);
                            }
                        }
                        else
                        {
                         
                            customer.id = customerTable.ContaAzulId.ToString();
                            customer.address.city.name = null;

                            var data1 = JsonConvert.SerializeObject(GetCustomerResponse[0]);
                            var data2 = JsonConvert.SerializeObject(customer);


                            var data = data2.Equals(data1);

                            if (!data1.Equals(data2))
                            {
                                //se ele já existe na tabela, só faz o update no conta azul
                                using (var customerCreation = new CustomerCreation(ContaAzulMiscSettings.UseSandbox))
                                    CustomerResponse = customerCreation.CreateAsyncUpdate(customer, customerTable.ContaAzulId.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();
                            }
                          
                        }
                    }
                    else
                    {//caso ele não exista no conta azul, faz a inserção dele no conta azul e no banco de dados

                        var data2 = JsonConvert.SerializeObject(customer);
                        using (var customerCreation = new CustomerCreation(ContaAzulMiscSettings.UseSandbox))
                            CustomerResponse = customerCreation.CreateAsync(customer, ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

                        if (CustomerResponse != null)
                        {
                            var customerContaAzul = new CustomerContaAzul();

                            customerContaAzul.ContaAzulId = CustomerResponse.id;
                            customerContaAzul.CustomerId = item.Id;
                            customerContaAzul.DataCriacao = DateTime.Now;
                            _contaAzulCustomerService.InsertCustomer(customerContaAzul);
                        }
                    }
                }
                catch (Exception ex)
                {
                           _logger.Error(ex.Message, ex);                         
                }

            }
        }


    }
}
