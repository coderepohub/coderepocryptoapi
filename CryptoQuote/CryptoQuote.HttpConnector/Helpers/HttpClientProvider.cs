using CryptoQuote.Contracts.HttpConnector;
using CryptoQuote.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace CryptoQuote.HttpConnector.Helpers
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;
        public HttpClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientSetting.ClientName);
        }

        ///<inheritdoc/>
        public async Task<HttpResponseMessage> GetJsonAsync(string uri)
        {
            return await _httpClient.GetAsync(uri);
        }
    }
}
