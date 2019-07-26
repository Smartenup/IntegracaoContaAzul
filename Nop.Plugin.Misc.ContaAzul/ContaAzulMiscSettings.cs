using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Misc.ContaAzul
{
    public class ContaAzulMiscSettings: ISettings
    {
        public bool UseSandbox { get; set; }

        public string RestAPISandBoxAccount { get; set; }

        public string RestAPIClientId { get; set; }

        public string RestAPISecrect { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }

        public string token { get; set; }

        public string redirect_uri { get; set; }

        public string scope { get; set; }

        public string state { get; set; }

        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string expires_in { get; set; }

        public string email { get; set; }

        public string senha { get; set; }

        public bool Log { get; set; }

        public string code { get; set; }

    }
}
