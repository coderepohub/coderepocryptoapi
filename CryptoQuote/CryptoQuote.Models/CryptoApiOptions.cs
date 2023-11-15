namespace CryptoQuote.Models
{
    public class CryptoApiOptions
    {
        public const string Name = "CryptoExchangeApi";

        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string GetCurrencyCodeUri { get; set; }
        public string GetExchangeUri { get; set; }
    }
}
