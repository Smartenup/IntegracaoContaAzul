using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ContaAzul.Models.Message.Request
{
    public class ProductMessage
    {
        //public ProductMessage()
        //{
        //    category = new Category();
        //}

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("value")]
        public decimal value { get; set; }

        [JsonProperty("cost")]
        public decimal cost { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("barcode")]
        public string barcode { get; set; }

        [JsonProperty("available_stock")]
        public decimal available_stock { get; set; }

        [JsonProperty("ncm_code")]
        public string ncm_code { get; set; }

        [JsonProperty("cest_code")]
        public string cest_code { get; set; }

        [JsonProperty("net_weight")]
        public decimal net_weight { get; set; }

        [JsonProperty("gross_weight")]
        public decimal gross_weight { get; set; }

        [JsonProperty("category_id")]
        public string category_id { get; set; }
    }

    //public partial class Category
    //{
    //    [JsonProperty("id")]
    //    public string id { get; set; }

    //    [JsonProperty("name")]
    //    public string name { get; set; }
    //}
}
