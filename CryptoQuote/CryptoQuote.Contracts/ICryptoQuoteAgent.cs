using CryptoQuote.Models;

namespace CryptoQuote.Contracts
{
    public interface ICryptoQuoteAgent
    {
        /// <summary>
        /// Gets list of Crypto currency codes.
        /// </summary>
        /// <returns>List of currency codes.</returns>
        Task<IEnumerable<CryptoCurrencyCodeResponse>> GetCryptoCurrencyCodesAsync(int limit);

        /// <summary>
        /// Get list of Crypto Currency Quotations.
        /// </summary>
        /// <param name="currencyCode">Currency Code e.g, BTC etc</param>
        /// <returns>List of Crypto currency quotations.</returns>
        Task<IEnumerable<CryptoCurrencyQuotation>> GetCryptoQuotationAsync(string currencyCode);
    }
}
