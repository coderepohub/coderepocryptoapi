using Newtonsoft.Json;

namespace CryptoQuote.Models
{
    public class CryptoCurrency
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("is_active")]
        public int IsActive { get; set; }

        [JsonProperty("first_historical_data")]
        public DateTime FirstHistoricalData { get; set; }

        [JsonProperty("last_historical_data")]
        public DateTime LastHistoricalData { get; set; }

        [JsonProperty("platform")]
        public object Platform { get; set; } // Assuming the "platform" field can be null
    }
}
