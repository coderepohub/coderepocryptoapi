namespace CryptoQuote.Models
{
    /// <summary>
    /// Get Crypto Currency Code response with Code and name
    /// </summary>
    public class CryptoCurrencyCodeResponse
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
    }
}
