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
                _logger.Log(LogLevel.Warning, $"{nameof(getCryptoCurrencyResult)} - GetCryptoCurrencyCodesAsync returns no data.");
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
                var cryptoQuotationResult = await _restClient.GetQuoteForCryptoAsync(currencyCode, exchangeCurrencyCode);
                if (cryptoQuotationResult is null || cryptoQuotationResult.Data is null)
                {
                    _logger.Log(LogLevel.Warning, $"{nameof(cryptoQuotationResult)} - GetQuoteForCryptoAsync returns no data for currency {exchangeCurrencyCode}");
                    continue;
                }


                // Get the currency details
                var cryptoData = cryptoQuotationResult.Data[currencyCode];
                if (cryptoData is null)
                {
                    _logger.Log(LogLevel.Warning, $"{nameof(cryptoData)} has no data for currency {exchangeCurrencyCode}");
                    continue;
                }
                var quote = cryptoData["quote"];
                if (quote is null)
                {
                    _logger.Log(LogLevel.Warning, $"{nameof(quote)} has no data for currency {exchangeCurrencyCode}");
                    continue;
                }
                var quotationDetails = quote[exchangeCurrencyCode];
                if (quotationDetails is null)
                {
                    _logger.Log(LogLevel.Warning, $"{nameof(quotationDetails)} has no data for currency {exchangeCurrencyCode}");
                    continue;
                }

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