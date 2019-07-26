using Newtonsoft.Json;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class CustomerCreation : APIResource
    {
        public CustomerCreation(bool sandBox) : base(sandBox)
        {
            BaseURI = "/customers";
        }

        public async Task<CustomerResponse> CreateAsync(object data, string token)
        {
            var retorno = await PostAsync<CustomerResponse>(data, null, token).ConfigureAwait(false);
            return retorno;
        }

        public async Task<CustomerResponse> CreateAsyncUpdate(CustomerMessage data, string id, string token)
        {
            var retorno = await PutAsync<CustomerResponse>(data, id, token).ConfigureAwait(false);
            return retorno;
        }
    }
}
