using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoQuote.Models
{
    public class CryptoCurrencyQuotationApiResponse
    {
        [JsonProperty("status")]
        public CryptoApiStatus Status { get; set; }

        [JsonProperty("data")]
        public JObject Data { get; set; }
    }
}
