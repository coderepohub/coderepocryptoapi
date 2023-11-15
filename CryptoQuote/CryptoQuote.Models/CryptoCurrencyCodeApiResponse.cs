using Newtonsoft.Json;

namespace CryptoQuote.Models
{
    public class CryptoCurrencyCodeApiResponse
    {
        [JsonProperty("status")]
        public CryptoApiStatus Status { get; set; }

        [JsonProperty("data")]
        public IEnumerable<CryptoCurrency> Data { get; set; }
    }
}
