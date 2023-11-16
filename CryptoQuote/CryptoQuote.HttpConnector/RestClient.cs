using CryptoQuote.Contracts.HttpConnector;
using CryptoQuote.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace CryptoQuote.HttpConnector
{
    public class RestClient : IRestClient
    {
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly CryptoApiOptions _cryptoApiOptions;

        public RestClient(IHttpClientProvider httpClientProvider, IOptions<CryptoApiOptions> options)
        {
            _cryptoApiOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
        }

        ///<inheritdoc/>
        public async Task<CryptoCurrencyCodeApiResponse> GetCryptoCurrencyCodesAsync(int limit)
        {
            if (limit <= 0)
            {
                throw new Exception($"limit must be greater than or equal to 1");
            }

            CryptoCurrencyCodeApiResponse cryptoCurrencyCodesResponse = null;
            string uri = $"{_cryptoApiOptions.GetCurrencyCodeUri}?limit={limit}";
            var httpResponseMessage = await SendRequest(Method.GET, uri);
            if (httpResponseMessage is not null)
            {
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseContent is null)
                    return cryptoCurrencyCodesResponse!;

                cryptoCurrencyCodesResponse = JsonConvert.DeserializeObject<CryptoCurrencyCodeApiResponse>(httpResponseContent)!;
            }

            return cryptoCurrencyCodesResponse!;
        }

        ///<inheritdoc/>
        public async Task<CryptoCurrencyQuotationApiResponse> GetQuoteForCryptoAsync(string cryptoCode, string convertCurrencyCode)
        {
            if (string.IsNullOrEmpty(cryptoCode)) throw new ArgumentNullException(nameof(cryptoCode));
            if (string.IsNullOrEmpty(convertCurrencyCode)) throw new ArgumentNullException(nameof(convertCurrencyCode));
            CryptoCurrencyQuotationApiResponse cryptoCurrencyQuoationApiResponse = null;
            string uri = $"{_cryptoApiOptions.GetExchangeUri}?symbol={cryptoCode}&convert={convertCurrencyCode}";
            var httpResponseMessage = await SendRequest(Method.GET, uri);
            if (httpResponseMessage is not null)
            {
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseContent is null)
                    return cryptoCurrencyQuoationApiResponse!;

                cryptoCurrencyQuoationApiResponse = JsonConvert.DeserializeObject<CryptoCurrencyQuotationApiResponse>(httpResponseContent)!;
            }

            return cryptoCurrencyQuoationApiResponse!;

        }


        #region Private Methods
        private async Task<HttpResponseMessage> SendRequest(Method method, string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException($"uri can not be null or empty");
            }

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                switch (method)
                {
                    case Method.GET:
                        response = await _httpClientProvider.GetJsonAsync(uri);
                        break;
                    default:
                        response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.MethodNotAllowed,
                        };
                        break;
                }
            }
            catch (Exception ex)
            {

                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
            return response;

        }

        #endregion
    }
}