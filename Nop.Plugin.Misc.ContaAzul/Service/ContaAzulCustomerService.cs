using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.ContaAzul.Service
{
    public class ContaAzulCustomerService: IContaAzulCustomerService
    {
        private readonly IRepository<CustomerContaAzul> _customerContaAzulRepository;

        public ContaAzulCustomerService(IRepository<CustomerContaAzul> customerContaAzulRepository)
        {
            _customerContaAzulRepository = customerContaAzulRepository;
        }

        public void InsertCustomer(CustomerContaAzul customerContaAzul)
        {
            if (customerContaAzul == null)
                throw new ArgumentNullException("customerContaAzul");

            _customerContaAzulRepository.Insert(customerContaAzul);
        }

        public void UpdateCustomer(CustomerContaAzul customerContaAzul)
        {
            throw new NotImplementedException();
        }

        public CustomerContaAzul GetCustomerContaAzul(Customer customer)
        {
            var query = _customerContaAzulRepository.Table;

            query = query.Where(o => o.CustomerId == customer.Id);

            if (query.Count() != 0)
                return query.FirstOrDefault();

            return null;
        }

        public CustomerContaAzul GetCustomer(CustomerResponse customer)
        {
            var query = _customerContaAzulRepository.Table;

            query = query.Where(o => o.ContaAzulId == customer.id);

            if (query.Count() != 0)
                return query.FirstOrDefault();

            return null;
        }
    }
}
