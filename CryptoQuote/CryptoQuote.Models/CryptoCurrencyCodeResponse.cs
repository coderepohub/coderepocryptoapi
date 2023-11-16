namespace CryptoQuote.Models
{
    /// <summary>
    /// Get Crypto Currency Code response with Code and name
    /// </summary>
    public class CryptoCurrencyCodeResponse
    {
        /// <summary>
        /// Crypto Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name of the Crypto currency
        /// </summary>
        public string Name { get; set; }
    }
}
