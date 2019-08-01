using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class GetCategory: APIResource
    {
        public GetCategory(bool sandBox) : base(sandBox)
        {
            BaseURI = "/product-categories";
        }

        public async Task<CategoryResponse[]> CreateAsync(string token, string filtro = null)
        {
                var retorno = await GetAsync<CategoryResponse[]>(null, filtro, token).ConfigureAwait(false);
                return retorno;           
        }
    }
}
