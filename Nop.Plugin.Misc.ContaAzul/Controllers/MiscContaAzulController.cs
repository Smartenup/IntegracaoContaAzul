using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Lib;
using Nop.Plugin.Misc.ContaAzul.Models;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using Nop.Plugin.Misc.ContaAzul.Service;
using Nop.Plugin.Misc.ContaAzul.Util.Helper;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ContaAzul.Controllers
{
    public class MiscContaAzulController : BasePluginController
    {
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IContaAzulCustomerService _contaAzulCustomerService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IContaAzulProductService _contaAzulProductService;

        public MiscContaAzulController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            PaymentSettings paymentSettings,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            ICustomerService customerService,
            IStoreContext storeContext,
            IOrderTotalCalculationService orderTotalCalculationService,
            IAddressAttributeParser addressAttributeParser,
            ITaxService taxService,
            IPriceCalculationService priceCalculationService,
            IContaAzulCustomerService contaAzulCustomerService,
            IProductService productService,
            ICategoryService categoryService,
            IContaAzulProductService contaAzulProductService)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _paymentService = paymentService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _paymentSettings = paymentSettings;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _customerService = customerService;
            _storeContext = storeContext;
            _orderTotalCalculationService = orderTotalCalculationService;
            _addressAttributeParser = addressAttributeParser;
            _taxService = taxService;
            _priceCalculationService = priceCalculationService;
            _contaAzulCustomerService = contaAzulCustomerService;
            _productService = productService;
            _categoryService = categoryService;
            _contaAzulProductService = contaAzulProductService;

            _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(string code = null, string state = null)
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);
            if (contaAzulMiscSettings == null) throw new ArgumentNullException(nameof(contaAzulMiscSettings));

            var model = new ConfigurationModel();
            model.email = contaAzulMiscSettings.email;
            model.senha = contaAzulMiscSettings.senha;
            model.client_id = contaAzulMiscSettings.client_id;
            model.client_secret = contaAzulMiscSettings.client_secret;
            model.code = code;
            model.state = state;      

            return View("~/Plugins/Misc.ContaAzul/Views/MiscContaAzul/Configure.cshtml", model);

        }

        public ActionResult GerarCodigo()
        {
            var state = Guid.NewGuid();
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            contaAzulMiscSettings.state = state.ToString();
            _settingService.SaveSetting(contaAzulMiscSettings);

            //now clear settings cache
            _settingService.ClearCache();
            return Redirect("https://api.contaazul.com/auth/authorize?redirect_uri=http://www.google.com&client_id=CFkrSgNEOYddgYvflcc39AydSJdDI950&scope=sales&state=" + state);

        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            //save settings
            contaAzulMiscSettings.email = model.email;
            contaAzulMiscSettings.senha = model.senha;
            contaAzulMiscSettings.client_id = model.client_id;
            contaAzulMiscSettings.client_secret = model.client_secret;
            contaAzulMiscSettings.code = model.code;

            _settingService.SaveSetting(contaAzulMiscSettings);

            //now clear settings cache
            _settingService.ClearCache();

            if (model.state != null && model.state == contaAzulMiscSettings.state)
            {
                GetToken();
                SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            }
            else
            {
                 ErrorNotification("State incompatível!" );                                   
            }

            return View("~/Plugins/Misc.ContaAzul/Views/MiscContaAzul/Configure.cshtml", model);


        }



        #region Token
        public void GetToken()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            string username = contaAzulMiscSettings.client_id;
            string password = contaAzulMiscSettings.client_secret;

            TokenResponse tokenResponse = null;

            try
            {
                using (var token = new Token(contaAzulMiscSettings.UseSandbox))
                    tokenResponse = token.CreateAsync(username, password, contaAzulMiscSettings.code).ConfigureAwait(false).GetAwaiter().GetResult();

                contaAzulMiscSettings.access_token = tokenResponse.AcessToken;
                contaAzulMiscSettings.refresh_token = tokenResponse.RefreshToken;

                _settingService.SaveSetting(contaAzulMiscSettings);

                //now clear settings cache
                _settingService.ClearCache();

            }
            catch (Exception ex)
            {
                ErrorNotification(ex.Message);
                _logger.Error(ex.Message, ex);
            }
        }

        public void RefreshToken()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            string username = contaAzulMiscSettings.client_id;
            string password = contaAzulMiscSettings.client_secret;

            TokenResponse tokenResponse = null;

            try
            {
                using (var token = new RefreshToken(contaAzulMiscSettings.UseSandbox))
                    tokenResponse = token.CreateAsync(username, password, contaAzulMiscSettings.refresh_token).ConfigureAwait(false).GetAwaiter().GetResult();

                contaAzulMiscSettings.access_token = tokenResponse.AcessToken;
                contaAzulMiscSettings.refresh_token = tokenResponse.RefreshToken;

                _settingService.SaveSetting(contaAzulMiscSettings);

                //now clear settings cache
                _settingService.ClearCache();
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.Message);
                _logger.Error(ex.Message, ex);
            }
        }
        #endregion


        #region getCustomerById
        public CustomerResponse getCustomerById(string id)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            CustomerResponse CustomerResponse = null;
            var customer = new CustomerMessage();
            customer.id = id;
            var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            try
            {
                using (var getcustomer = new GetCustomer(ContaAzulMiscSettings.UseSandbox))
                    CustomerResponse = getcustomer.CreateAsyncById(customer, ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                try
                {
                    var retorno = JsonConvert.DeserializeObject<MiscExecutitionResponse>(ex.Message, ConverterPaymentExecution.Settings);

                    if (retorno.StatusCode == "401")
                    {
                        RefreshToken();
                        _logger.Error("Token expirado " + ContaAzulMiscSettings.access_token, ex);

                    }
                    else
                    {
                        _logger.Error(ex.Message, ex);

                    }
                }
                catch (Exception erro)
                {
                    _logger.Error(erro.Message, erro);

                    throw;
                }
            }

            return CustomerResponse;
        }
        #endregion

        #region updateCustomer
        public CustomerResponse UpdateCustomer()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            CustomerResponse CustomerResponse = null;
            var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);
            var customerPayPalPlus = _contaAzulCustomerService.GetCustomerContaAzul(customer);
            var customerMessage = new CustomerMessage();

            var number = string.Empty;
            var complement = string.Empty;
            var cpfCnpj = string.Empty;

            customerMessage.name = customer.BillingAddress != null ? customer.BillingAddress.FirstName + " " + customer.BillingAddress.LastName : "";
            customerMessage.companyName = customer.BillingAddress != null ? customer.BillingAddress.Company : null;
            customerMessage.email = customer.Email;
            customerMessage.mobilePhone = customer.BillingAddress != null ? customer.BillingAddress.PhoneNumber : null;
            customerMessage.address.city.name = customer.BillingAddress != null ? customer.BillingAddress.City : null;
            customerMessage.address.state.name = customer.BillingAddress != null ? customer.BillingAddress.StateProvince != null ? customer.BillingAddress.StateProvince.Name : null : null;
            // customer.address.zipCode = item.BillingAddress != null ? item.BillingAddress.ZipPostalCode : null;
            customerMessage.address.street = customer.BillingAddress != null ? customer.BillingAddress.Address1 : null;
            customerMessage.address.complement = complement;
            customerMessage.address.number = number;
            customerMessage.document = "";

            try
            {
                using (var customerCreation = new CustomerCreation(ContaAzulMiscSettings.UseSandbox))
                    CustomerResponse = customerCreation.CreateAsyncUpdate(customerMessage, customerPayPalPlus.ContaAzulId.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                try
                {
                    var retorno = JsonConvert.DeserializeObject<MiscExecutitionResponse>(ex.Message, ConverterPaymentExecution.Settings);

                    if (retorno.StatusCode == "401")
                    {
                        RefreshToken();
                        _logger.Error("Token expirado " + ContaAzulMiscSettings.access_token, ex);

                    }
                    else
                    {
                        _logger.Error(ex.Message, ex);

                    }
                }
                catch (Exception erro)
                {
                    _logger.Error(erro.Message, erro);

                    throw;
                }

            }

            return CustomerResponse;
        }
        #endregion

        #region InsertCustomer
        public CustomerResponse InsertCustomer(Customer customer)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            CustomerResponse CustomerResponse = null;
            var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);

            var customerMessage = new CustomerMessage();

            var number = string.Empty;
            var complement = string.Empty;
            var cpfCnpj = string.Empty;

            customerMessage.name = customer.BillingAddress != null ? customer.BillingAddress.FirstName + " " + customer.BillingAddress.LastName : "";
            customerMessage.companyName = customer.BillingAddress != null ? customer.BillingAddress.Company : null;
            customerMessage.email = customer.Email;
            customerMessage.mobilePhone = customer.BillingAddress != null ? customer.BillingAddress.PhoneNumber : null;
            customerMessage.address.city.name = customer.BillingAddress != null ? customer.BillingAddress.City : null;
            customerMessage.address.state.name = customer.BillingAddress != null ? customer.BillingAddress.StateProvince != null ? customer.BillingAddress.StateProvince.Name : null : null;
            // customer.address.zipCode = item.BillingAddress != null ? item.BillingAddress.ZipPostalCode : null;
            customerMessage.address.street = customer.BillingAddress != null ? customer.BillingAddress.Address1 : null;
            customerMessage.address.complement = complement;
            customerMessage.address.number = number;

            try
            {
                using (var customerCreation = new CustomerCreation(ContaAzulMiscSettings.UseSandbox))
                    CustomerResponse = customerCreation.CreateAsync(customerMessage, ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                try
                {
                    var retorno = JsonConvert.DeserializeObject<MiscExecutitionResponse>(ex.Message, ConverterPaymentExecution.Settings);

                    if (retorno.StatusCode == "401")
                    {
                        RefreshToken();
                        _logger.Error("Token expirado " + ContaAzulMiscSettings.access_token, ex);

                    }
                    else
                    {
                        _logger.Error(ex.Message, ex);

                    }
                }
                catch (Exception erro)
                {
                    _logger.Error(erro.Message, erro);

                    throw;
                }

            }

            return CustomerResponse;
        }
        #endregion

        #region SincronizaContatos
        public void SincronizaContatos()
        {
            var customers = _customerService.GetAllCustomers();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            CustomerResponse[] GetCustomerResponse = null;
            CustomerResponse CustomerResponse = null;
            var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>(storeScope);


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
                customer.mobilePhone = item.BillingAddress != null ? item.BillingAddress.PhoneNumber : null;
                customer.address.city.name = item.BillingAddress != null ? item.BillingAddress.City : null;
                customer.address.state.name = item.BillingAddress != null ? item.BillingAddress.StateProvince != null ? item.BillingAddress.StateProvince.Name : null : null;
                // customer.address.zipCode = item.BillingAddress != null ? item.BillingAddress.ZipPostalCode : null;
                customer.address.street = item.BillingAddress != null ? item.BillingAddress.Address1 : null;
                customer.address.complement = complement;
                customer.address.number = number;
                customer.document = cpfCnpj;

                try
                {
                    var filtro = "?search=" + cpfCnpj;
                    using (var getcustomer = new GetCustomer(ContaAzulMiscSettings.UseSandbox))
                        GetCustomerResponse = getcustomer.CreateAsync(null, ContaAzulMiscSettings.access_token, filtro).ConfigureAwait(false).GetAwaiter().GetResult();
                    //busca por cpf conta azul, se existir, verifica se já foi adicionado na tabela do banco
                    if (GetCustomerResponse != null)
                    {
                        var customerPayPalPlus = _contaAzulCustomerService.GetCustomer(GetCustomerResponse[0]);
                        //caso ele não exista na tabela relacional do banco, insere e atualiza no conta azul
                        if (customerPayPalPlus == null)
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
                            //se ele já existe na tabela, só faz o update no conta azul
                            using (var customerCreation = new CustomerCreation(ContaAzulMiscSettings.UseSandbox))
                                CustomerResponse = customerCreation.CreateAsyncUpdate(customer, customerPayPalPlus.ContaAzulId.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                    }
                    else
                    {//caso ele não exista no conta azul, faz a inserção dele no conta azul e no banco de dados
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
                    try
                    {
                        var retorno = JsonConvert.DeserializeObject<MiscExecutitionResponse>(ex.Message, ConverterPaymentExecution.Settings);

                        if (retorno.StatusCode == "401")
                        {
                            RefreshToken();
                            _logger.Error("Token expirado " + ContaAzulMiscSettings.access_token, ex);

                        }
                        else
                        {
                            _logger.Error(ex.Message, ex);

                        }
                    }
                    catch (Exception erro)
                    {
                        _logger.Error(erro.Message, erro);

                        throw;
                    }

                    // ErrorNotification("O Customer com id " + item.Id + " não foi encontrado" );                                   
                }

            }

        }
        #endregion

        #region SincronizaProdutos
        public void SincronizaProdutos()
        {
            RefreshToken();

            var products = _productService.SearchProducts(categoryIds: null,
                pageSize: int.MaxValue,
                showHidden: true);

            ProductResponse[] GetProductResponse = null;
            ProductResponse ProductResponse = null;
            var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>();

            foreach (var item in products)
            {
                var product = new ProductMessage();
                //var categoria = _categoryService.GetProductCategoriesByProductId(item.Id).Select(x => x.CategoryId).FirstOrDefault();
                var categoria = _categoryService.GetProductCategoryById(item.Id);

                product.name = item.Name;
                product.value = item.Price;
                product.cost = item.ProductCost;
                product.available_stock = item.StockQuantity;
                product.net_weight = Math.Round(item.Weight, 2);
                product.category.id = categoria.Category.Id.ToString();
                product.category.name = categoria.Category.Name;


                try
                {
                    var filtro = "?name=" + item.Name;

                    using (var getproduct = new GetProduct(ContaAzulMiscSettings.UseSandbox))
                        GetProductResponse = getproduct.CreateAsync(null, ContaAzulMiscSettings.access_token, filtro).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (GetProductResponse.Count() > 0)
                    {
                        var productTable = _contaAzulProductService.GetProduct(GetProductResponse[0]);
                        //caso ele não exista na tabela relacional do banco, insere e atualiza no conta azul
                        if (productTable == null)
                        {
                            using (var productCreation = new ProductCreation(ContaAzulMiscSettings.UseSandbox))
                                ProductResponse = productCreation.CreateAsyncUpdate(product, GetProductResponse[0].id.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

                            if (ProductResponse != null)
                            {
                                var productContaAzul = new ProductContaAzul();

                                productContaAzul.ContaAzulId = ProductResponse.id;
                                productContaAzul.ProductId = item.Id;
                                productContaAzul.DataCriacao = DateTime.Now;
                                _contaAzulProductService.InsertProduct(productContaAzul);
                            }

                        }else
                        {//caso já existe no banco, só verifica se houve alteração e faz o update no conta azul

                            product.id = productTable.ContaAzulId.ToString();

                            var objetoContaAzul = JsonConvert.SerializeObject(GetProductResponse[0]);
                            var objetoAtualizar = JsonConvert.SerializeObject(product);

                            var data = objetoAtualizar.Equals(objetoContaAzul);

                            if (!objetoAtualizar.Equals(objetoContaAzul))
                            {
                                using (var productCreation = new ProductCreation(ContaAzulMiscSettings.UseSandbox))
                                    ProductResponse = productCreation.CreateAsyncUpdate(product, productTable.ContaAzulId.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();
                            }
                        }
                    }
                    else
                    {//caso ele não exista no conta azul, faz a inserção dele no conta azul e no banco de dados
                        var data2 = JsonConvert.SerializeObject(product);

                        using (var productCreation = new ProductCreation(ContaAzulMiscSettings.UseSandbox))
                            ProductResponse = productCreation.CreateAsync(product, ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

                        if (ProductResponse != null)
                        {
                            var productContaAzul = new ProductContaAzul();

                            productContaAzul.ContaAzulId = ProductResponse.id;
                            productContaAzul.ProductId = item.Id;
                            productContaAzul.DataCriacao = DateTime.Now;
                            _contaAzulProductService.InsertProduct(productContaAzul);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);

                }
            }

        }
        #endregion
    }
}
