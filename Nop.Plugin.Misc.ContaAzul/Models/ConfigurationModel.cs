using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.ContaAzul.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.RestAPISandBoxAccount")]
        public string RestAPISandBoxAccount { get; set; }
        public bool RestAPISandBoxAccount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.RestAPIClientId")]
        public string RestAPIClientId { get; set; }
        public bool RestAPIClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.RestAPISecrect")]
        public string RestAPISecrect { get; set; }
        public bool RestAPISecrect_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.ClientId")]
        public string client_id { get; set; }
        public bool client_id_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.ClientSecret")]
        public string client_secret { get; set; }
        public bool client_secret_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.Token")]
        public string token { get; set; }
        public bool token_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.RedirectUri")]
        public string redirect_uri { get; set; }
        public bool redirect_uri_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.Scope")]
        public string scope { get; set; }
        public bool scope_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.State")]
        public string state { get; set; }
        public bool state_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.AccessToken")]
        public string access_token { get; set; }
        public bool access_token_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.RefreshToken")]
        public string refresh_token { get; set; }
        public bool refresh_token_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.ExpiresIn")]
        public string expires_in { get; set; }
        public bool expires_in_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.Email")]
        public string email { get; set; }
        public bool email_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.Senha")]
        public string senha { get; set; }
        public bool senha_OverrideForStore { get; set; }

        public bool Log { get; set; }
        public bool Log_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.IntegracaoContaAzul.Fields.Code")]
        public string code { get; set; }
        public bool code_OverrideForStore { get; set; }


    }
}
