using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class ProductCreation: APIResource
    {
        public ProductCreation(bool sandBox) : base(sandBox)
        {
            BaseURI = "/products";
        }

        public async Task<ProductResponse> CreateAsync(object data, string token)
        {
            var retorno = await PostAsync<ProductResponse>(data, null, token).ConfigureAwait(false);
            return retorno;
        }

        public async Task<ProductResponse> CreateAsyncUpdate(ProductMessage data, string id, string token)
        {
            var retorno = await PutAsync<ProductResponse>(data, id, token).ConfigureAwait(false);
            return retorno;
        }
    }
}
