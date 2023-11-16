using CryptoQuote.Contracts.HttpConnector;
using CryptoQuote.HttpConnector;
using CryptoQuote.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace CryptoQuote.Tests
{
    public class RestClientTests
    {
        private readonly Mock<IHttpClientProvider> _mockHttpClientProvider;
        private readonly Mock<IOptions<CryptoApiOptions>> _mockCryptoApiOption;
        private readonly IRestClient _restClient;
        public RestClientTests()
        {
            _mockHttpClientProvider = new Mock<IHttpClientProvider>();
            _mockCryptoApiOption = new Mock<IOptions<CryptoApiOptions>>();
            _mockCryptoApiOption.Setup(o => o.Value).Returns(new CryptoApiOptions
            {
                GetCurrencyCodeUri = "https://api.crypto.com/currency",
                GetExchangeUri = "https://api.crypto.com/exchange"
            });
            _restClient = new RestClient(_mockHttpClientProvider.Object, _mockCryptoApiOption.Object);
        }


        #region Test for Constructor

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenOptionsIsNull()
        {
            // Act
            Action act = () => new RestClient(_mockHttpClientProvider.Object, null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*options*");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientProviderIsNull()
        {
            // Act
            Action act = () => new RestClient(null, _mockCryptoApiOption.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*httpClientProvider*");
        }
        #endregion


        #region Test for GetCryptoCurrencyCodesAsync method

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetCryptoCurrencyCodesAsync_ShouldReturnCryptoCurrencyCodeApiResponse_WhenHttpResponseIsSuccessful(int limit)
        {
            // Arrange
            var expectedResponse = new CryptoCurrencyCodeApiResponse
            {
                Status = new CryptoApiStatus
                {
                    ErrorCode = 0,
                    ErrorMessage = null
                },
                Data = new List<CryptoCurrency>
                {
                        new CryptoCurrency { Symbol = "BTC", Name = "Bitcoin" },
                        new CryptoCurrency { Symbol = "ETH", Name = "Ethereum" }
                }
            };
            var expectedJson = JsonConvert.SerializeObject(expectedResponse);
            var expectedUri = $"https://api.crypto.com/currency?limit={limit}";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedJson)
            };
            _mockHttpClientProvider.Setup(h => h.GetJsonAsync(expectedUri)).ReturnsAsync(httpResponseMessage);

            // Act
            var actualResponse = await _restClient.GetCryptoCurrencyCodesAsync(limit);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }


        [Fact]
        public async Task GetCryptoCurrencyCodesAsync_ShouldReturnNull_WhenHttpResponseIsNull()
        {
            // Arrange
            var limit = 10;
            var expectedUri = $"https://api.crypto.com/currency?limit={limit}";
            _mockHttpClientProvider.Setup(h => h.GetJsonAsync(expectedUri)).ReturnsAsync((HttpResponseMessage)null);

            // Act
            var actualResponse = await _restClient.GetCryptoCurrencyCodesAsync(limit);

            // Assert
            actualResponse.Should().BeNull();
        }


        [Fact]
        public async Task GetCryptoCurrencyCodesAsync_ShouldReturnNull_WhenHttpResponseContentIsNull()
        {
            // Arrange
            var limit = 10;
            var expectedUri = $"https://api.crypto.com/currency?limit={limit}";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null
            };
            _mockHttpClientProvider.Setup(h => h.GetJsonAsync(expectedUri)).ReturnsAsync(httpResponseMessage);

            // Act
            var actualResponse = await _restClient.GetCryptoCurrencyCodesAsync(limit);

            // Assert
            actualResponse.Should().BeNull();
        }

        #endregion


        #region Test for GetQuoteForCryptoAsync
        [Fact]
        public async Task GetQuoteForCryptoAsync_WithNullOrEmptyCryptoCode_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _restClient.GetQuoteForCryptoAsync(null, "USD")
            );
        }

        [Fact]
        public async Task GetQuoteForCryptoAsync_WithNullOrEmptyConvertCurrencyCode_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _restClient.GetQuoteForCryptoAsync("BTC", null)
            );
        }

        [Theory]
        [InlineData("BTC", "USD")]
        [InlineData("ETH", "EUR")]
        public async Task GetQuoteForCryptoAsync_ShouldReturnCryptoCurrencyQuotationApiResponse_WhenHttpResponseIsSuccessful(string cryptoCode, string convertCurrencyCode)
        {
            // Arrange
            var expectedResponse = new CryptoCurrencyQuotationApiResponse
            {
                Status = new CryptoApiStatus
                {
                    ErrorCode = 0,
                    ErrorMessage = null
                },
                Data = new JObject
                {
                    ["quote"] = new JObject
                    {
                        ["price"] = 1000.00m,
                        ["lastUpdated"] = DateTime.Now
                    }
                }
            };
            var expectedJson = JsonConvert.SerializeObject(expectedResponse);
            var expectedUri = $"https://api.crypto.com/exchange?symbol={cryptoCode}&convert={convertCurrencyCode}";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedJson)
            };
            _mockHttpClientProvider.Setup(h => h.GetJsonAsync(expectedUri)).ReturnsAsync(httpResponseMessage);

            // Act
            var actualResponse = await _restClient.GetQuoteForCryptoAsync(cryptoCode, convertCurrencyCode);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            actualResponse.Status.Should().NotBeNull();
            actualResponse.Status.ErrorCode.Should().Be(0);
            actualResponse.Status.ErrorMessage.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetQuoteForCryptoAsync_ShouldReturnNull_WhenHttpResponseIsNull()
        {
            // Arrange
            string cryptoCode = "BTC";
            string convertCurrencyCode = "USD";
            var expectedUri = $"https://api.crypto.com/exchange?symbol={cryptoCode}&convert={convertCurrencyCode}";
            _mockHttpClientProvider.Setup(h => h.GetJsonAsync(expectedUri)).ReturnsAsync((HttpResponseMessage)null);

            // Act
            var actualResponse = await _restClient.GetQuoteForCryptoAsync(cryptoCode,convertCurrencyCode);

            // Assert
            actualResponse.Should().BeNull();
        }


        [Fact]
        public async Task GetQuoteForCryptoAsync_ShouldReturnNull_WhenHttpResponseContentIsNull()
        {
            // Arrange
            // Arrange
            string cryptoCode = "BTC";
            string convertCurrencyCode = "USD";
            var expectedUri = $"https://api.crypto.com/exchange?symbol={cryptoCode}&convert={convertCurrencyCode}";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null
            };
            _mockHttpClientProvider.Setup(h => h.GetJsonAsync(expectedUri)).ReturnsAsync(httpResponseMessage);

            // Act
            var actualResponse = await _restClient.GetQuoteForCryptoAsync(cryptoCode, convertCurrencyCode);

            // Assert
            actualResponse.Should().BeNull();
        }

        #endregion
    }
}