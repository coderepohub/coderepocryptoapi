using AutoMapper;
using CryptoQuote.Contracts;
using CryptoQuote.Contracts.HttpConnector;
using CryptoQuote.Models;
using Microsoft.Extensions.Logging;

namespace CryptoQuote.Agent
{
    public class CryptoQuoteAgent : ICryptoQuoteAgent
    {
        private readonly IRestClient _restClient;
        private readonly ILogger<CryptoQuoteAgent> _logger;
        private readonly IMapper _mapper;
        public CryptoQuoteAgent(IRestClient restClient, ILogger<CryptoQuoteAgent> logger, IMapper mapper)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<CryptoCurrencyCodeResponse>> GetCryptoCurrencyCodesAsync(int limit)
        {
            var getCryptoCurrencyResult = await _restClient.GetCryptoCurrencyCodesAsync(limit);
            if (getCryptoCurrencyResult is null || getCryptoCurrencyResult.Data is null)
            {
                return Enumerable.Empty<CryptoCurrencyCodeResponse>();
            }

            return _mapper.Map<IEnumerable<CryptoCurrencyCodeResponse>>(getCryptoCurrencyResult.Data);

        }

        ///<inheritdoc/>
        public async Task<IEnumerable<CryptoCurrencyQuotation>> GetCryptoQuotationAsync(string currencyCode)
        {
            List<string> exchangeCurrencyCodes = new List<string> { "USD", "EUR", "BRL", "GBP", "AUD" };
            List<CryptoCurrencyQuotation> cryptoCurrencies = new();

            foreach (var exchangeCurrencyCode in exchangeCurrencyCodes)
            {
                var cryptoQuotationResult = await _restClient.GetQuoteForCurrenciesAsync(currencyCode, exchangeCurrencyCode);
                if (cryptoQuotationResult is null || cryptoQuotationResult.Data is null)
                    continue;

                // Get the currency details
                var cryptoData = cryptoQuotationResult.Data[currencyCode];
                if (cryptoData is null) continue;
                var quote = cryptoData["quote"];
                if (quote is null) continue;
                var quotationDetails = quote[exchangeCurrencyCode];
                if (quotationDetails is null) continue;

                CryptoCurrencyQuotation cryptoCurrencyQuotation = new CryptoCurrencyQuotation
                {
                    Currency = exchangeCurrencyCode
                };
                double.TryParse(quotationDetails["price"]?.ToString(), out double price);
                cryptoCurrencyQuotation.Price = price;

                cryptoCurrencies.Add(cryptoCurrencyQuotation);
            }
            return cryptoCurrencies;
        }
    }
}