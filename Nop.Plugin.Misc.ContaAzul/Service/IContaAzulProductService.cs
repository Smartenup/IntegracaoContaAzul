using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;

namespace Nop.Plugin.Misc.ContaAzul.Service
{
    public interface IContaAzulProductService
    {
        void InsertProduct(ProductContaAzul productContaAzul);

        ProductContaAzul GetProduct(ProductResponse customer);


    }
}
