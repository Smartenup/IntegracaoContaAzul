using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class Token : APIResource
    {
        public Token(bool sandBox) : base(sandBox)
        {
            BaseURI = "/oauth2/token";
        }

        public async Task<TokenResponse> CreateAsync(string username, string password,string code)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "redirect_uri","http://www.google.com"},
                { "code", code }
            };

            var content = new FormUrlEncodedContent(dictionary);

            var retorno = await PostAsync<TokenResponse>(null, null, null, username, password, content).ConfigureAwait(false);
            return retorno;
        }


    }
}
