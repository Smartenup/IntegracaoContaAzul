using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class GetCustomer: APIResource
    {
        public GetCustomer(bool sandBox) : base(sandBox)
        {
            BaseURI = "/customers";
        }

        public async Task<CustomerResponse[]> CreateAsync(CustomerMessage customerMessage, string token, string filtro = null)
        {
            if(filtro != null)
            {
                var retorno = await GetAsync<CustomerResponse[]>(null, filtro, token).ConfigureAwait(false);
                return retorno;
            }
            else
            {
                var retorno = await GetAsync<CustomerResponse[]>(customerMessage.id, null, token).ConfigureAwait(false);
                return retorno;
            }
        }

        public async Task<CustomerResponse> CreateAsyncById(CustomerMessage customerMessage, string token, string filtro = null)
        {
           
                var retorno = await GetAsync<CustomerResponse>(customerMessage.id, null, token).ConfigureAwait(false);
                return retorno;
        }



    }
}
