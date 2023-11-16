using CryptoQuote.Models;

namespace CryptoQuote.Contracts.HttpConnector;

public interface IRestClient
{
    /// <summary>
    /// Returns List of Curerency Codes.
    /// </summary>
    /// <returns>CryptoCurrencyCodes Response</returns>
    Task<CryptoCurrencyCodeApiResponse> GetCryptoCurrencyCodesAsync(int limit);

    /// <summary>
    /// Returns Quoation for currency passed
    /// </summary>
    /// <param name="cryptoCode">Crypto currency code</param>
    /// <param name="convertCurrencyCode">Currency code in which conversion needs to be calculated</param>
    /// <returns>Returns the quoation in USD,EUR,BRL,GBP,AUD</returns>
    Task<CryptoCurrencyQuotationApiResponse> GetQuoteForCryptoAsync(string cryptoCode, string convertCurrencyCode);
}