using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Models.Message.Response
{
    public partial class MiscExecutitionResponse
    {
        [JsonProperty("StatusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("ReasonPhase")]
        public string ReasonPhase { get; set; }

        [JsonProperty("Message")]
        public Message Message { get; set; }
     
    }

    public partial class Message
    {
        [JsonProperty("Message")]
        public string Errors { get; set; }
    }

    public partial class MiscExecutionResponse
    {
        public static MiscExecutionResponse FromJson(string json) => JsonConvert.DeserializeObject<MiscExecutionResponse>(json, ConverterPaymentExecution.Settings);
    }


    internal static class ConverterPaymentExecution
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
