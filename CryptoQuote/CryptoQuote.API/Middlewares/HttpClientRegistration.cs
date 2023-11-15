using CryptoQuote.Models;
using Microsoft.Extensions.Options;

namespace CryptoQuote.API.Middlewares
{
    public static class HttpClientRegistration
    {
        public static IServiceCollection AddCryptoHttpClient(this IServiceCollection services)
        {

            services.AddHttpClient(HttpClientSetting.ClientName, (serviceProvider, httpClient) =>
            {
                var cryptoApiOptions = serviceProvider.GetRequiredService<IOptions<CryptoApiOptions>>().Value;
                _ = httpClient.BaseAddress = new Uri(cryptoApiOptions.BaseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", cryptoApiOptions.ApiKey);
            });

            return services;
        }
    }
}
