using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ContaAzul.Models.Message.Response
{
    public class CategoryResponse
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }
    }
}
