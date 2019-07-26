using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class GetProduct: APIResource
    {
        public GetProduct(bool sandBox) : base(sandBox)
        {
            BaseURI = "/products";
        }

        public async Task<ProductResponse[]> CreateAsync(ProductMessage productMessage, string token, string filtro = null)
        {
            if (filtro != null)
            {
                var retorno = await GetAsync<ProductResponse[]>(null, filtro, token).ConfigureAwait(false);
                return retorno;
            }
            else
            {
                var retorno = await GetAsync<ProductResponse[]>(productMessage.id, null, token).ConfigureAwait(false);
                return retorno;
            }
        }
    }
}
