using CryptoQuote.API.Middlewares;
using CryptoQuote.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoQuote.Tests.MiddlewareTests
{
    public class HttpClientRegistrationTests
    {
        [Fact]
        public void AddCryptoHttpClient_Should_Register_HttpClient_With_CryptoApiOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var cryptoApiOptions = new CryptoApiOptions
            {
                BaseUrl = "https://example.com",
                ApiKey = "123456"
            };
            services.Configure<CryptoApiOptions>(options =>
            {
                options.BaseUrl = cryptoApiOptions.BaseUrl;
                options.ApiKey = cryptoApiOptions.ApiKey;
            });

            // Act
            services.AddCryptoHttpClient();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(HttpClientSetting.ClientName);
            httpClient.BaseAddress.Should().Be(new Uri(cryptoApiOptions.BaseUrl));
            httpClient.DefaultRequestHeaders.Accept.Should().ContainSingle(header => header.MediaType == "application/json");
            httpClient.DefaultRequestHeaders.Should().ContainKey("X-CMC_PRO_API_KEY")
                .WhoseValue.Should().ContainSingle(headerValue => headerValue == cryptoApiOptions.ApiKey);
        }
    }
}
