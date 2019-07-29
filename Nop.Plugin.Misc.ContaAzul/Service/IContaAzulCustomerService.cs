using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;

namespace Nop.Plugin.Misc.ContaAzul.Service
{
    public interface IContaAzulCustomerService
    {
        void InsertCustomer(CustomerContaAzul customerContaAzul);

        CustomerContaAzul GetCustomerContaAzul(Customer customer);

        CustomerContaAzul GetCustomer(CustomerResponse customer);
    }
}
