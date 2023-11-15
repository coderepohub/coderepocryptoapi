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
    /// <param name="currencyCode"></param>
    /// <returns>Returns the quoation in USD,EUR,BRL,GBP,AUD</returns>
    Task<CryptoCurrencyQuotationApiResponse> GetQuoteForCurrenciesAsync(string currencyCode, string convertCode);
}