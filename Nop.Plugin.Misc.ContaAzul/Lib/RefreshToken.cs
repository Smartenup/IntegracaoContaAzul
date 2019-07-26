using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Lib
{
    public class RefreshToken: APIResource
    {
        public RefreshToken(bool sandBox) : base(sandBox)
        {
            BaseURI = "/oauth2/token";
        }

        public async Task<TokenResponse> CreateAsync(string username, string password, string refreshToken)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken}
            };

            var content = new FormUrlEncodedContent(dictionary);

            var retorno = await PostAsync<TokenResponse>(null, null, null, username, password, content).ConfigureAwait(false);
            return retorno;
        }
    }
}
