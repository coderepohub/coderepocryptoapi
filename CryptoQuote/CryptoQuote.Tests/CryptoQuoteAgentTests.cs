using AutoMapper;
using CryptoQuote.Agent;
using CryptoQuote.Contracts;
using CryptoQuote.Contracts.HttpConnector;
using CryptoQuote.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace CryptoQuote.Tests
{
    public class CryptoQuoteAgentTests
    {
        private readonly Mock<IRestClient> _mockRestClient;
        private readonly Mock<ILogger<CryptoQuoteAgent>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ICryptoQuoteAgent _cryptoQuoteAgent;
        public CryptoQuoteAgentTests()
        {
            _mockRestClient = new Mock<IRestClient>();
            _mockLogger = new Mock<ILogger<CryptoQuoteAgent>>();
            _mockMapper = new Mock<IMapper>();
            _cryptoQuoteAgent = new CryptoQuoteAgent(_mockRestClient.Object, _mockLogger.Object, _mockMapper.Object);
        }


        #region Test for Constructor

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRestlientIsNull()
        {
            // Act
            Action act = () => new CryptoQuoteAgent(null, _mockLogger.Object, _mockMapper.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*restClient*");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            // Act
            Action act = () => new CryptoQuoteAgent(_mockRestClient.Object, null, _mockMapper.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*logger*");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            // Act
            Action act = () => new CryptoQuoteAgent(_mockRestClient.Object, _mockLogger.Object, null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*mapper*");
        }
        #endregion

        #region Tests for GetCryptoCurrencyCodesAsync
        [Theory]
        [InlineData(100)]
        public async Task GetCryptoCurrencyCodesAsync_ShouldReturnMappedResult_WhenRestClientReturnsData(int limit)
        {
            // Arrange
            string warningMessage = $"getCryptoCurrencyResult - GetCryptoCurrencyCodesAsync returns no data.";
            var restClientResult = new CryptoCurrencyCodeApiResponse
            {
                Data = new List<CryptoCurrency>
                {
                    new CryptoCurrency { Id = 1, Name = "Bitcoin", Symbol = "BTC" },
                    new CryptoCurrency { Id = 2, Name = "Ethereum", Symbol = "ETH" }
                }
            };
            var mappedResult = new List<CryptoCurrencyCodeResponse>
            {
                new CryptoCurrencyCodeResponse {Name = "Bitcoin", Code = "BTC" },
                new CryptoCurrencyCodeResponse { Name = "Ethereum", Code = "ETH" }
            };
            _mockRestClient.Setup(x => x.GetCryptoCurrencyCodesAsync(limit)).ReturnsAsync(restClientResult);
            _mockMapper.Setup(x => x.Map<IEnumerable<CryptoCurrencyCodeResponse>>(restClientResult.Data)).Returns(mappedResult);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoCurrencyCodesAsync(limit);

            // Assert
            result.Should().BeEquivalentTo(mappedResult);
            _mockRestClient.Verify(x => x.GetCryptoCurrencyCodesAsync(limit), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<CryptoCurrencyCodeResponse>>(restClientResult.Data), Times.Once);

            _mockLogger.Verify(x => x.Log(
       LogLevel.Warning,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString() == warningMessage),
        It.IsAny<Exception>(),
        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);


        }


        [Fact]
        public async Task GetCryptoCurrencyCodesAsync_ShouldReturnEmptyResult_WhenRestClientReturnsNull()
        {
            // Arrange
            string warningMessage = $"getCryptoCurrencyResult - GetCryptoCurrencyCodesAsync returns no data.";
            _mockRestClient.Setup(x => x.GetCryptoCurrencyCodesAsync(It.IsAny<int>())).ReturnsAsync((CryptoCurrencyCodeApiResponse)null);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoCurrencyCodesAsync(10);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetCryptoCurrencyCodesAsync(10), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<CryptoCurrencyCodeResponse>>(It.IsAny<IEnumerable<CryptoCurrency>>()), Times.Never);

            _mockLogger.Verify(x => x.Log(
      LogLevel.Warning,
       It.IsAny<EventId>(),
       It.Is<It.IsAnyType>((v, t) => v.ToString() == warningMessage),
       It.IsAny<Exception>(),
       (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

        }

        [Fact]
        public async Task GetCryptoCurrencyCodesAsync_ShouldReturnEmptyResult_WhenRestClientReturnsDataNull()
        {
            // Arrange
            string warningMessage = $"getCryptoCurrencyResult - GetCryptoCurrencyCodesAsync returns no data.";
            var restClientResult = new CryptoCurrencyCodeApiResponse
            {
                Data = null
            };
            _mockRestClient.Setup(x => x.GetCryptoCurrencyCodesAsync(It.IsAny<int>())).ReturnsAsync(restClientResult);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoCurrencyCodesAsync(10);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetCryptoCurrencyCodesAsync(10), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<CryptoCurrencyCodeResponse>>(It.IsAny<IEnumerable<CryptoCurrency>>()), Times.Never);

            _mockLogger.Verify(x => x.Log(
     LogLevel.Warning,
      It.IsAny<EventId>(),
      It.Is<It.IsAnyType>((v, t) => v.ToString() == warningMessage),
      It.IsAny<Exception>(),
      (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
        #endregion


        #region Test for GetCryptoQuotationAsync
        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnQuotations_WhenRestClientReturnsData(string currencyCode)
        {
            // Arrange
            string warningMessage = $"cryptoQuotationResult - GetQuoteForCryptoAsync returns no data.";
            var restClientResultForUSD = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["USD"] = new JObject
                            {
                                ["price"] = 50000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForEUR = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["EUR"] = new JObject
                            {
                                ["price"] = 42000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForBRL = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["BRL"] = new JObject
                            {
                                ["price"] = 270000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForGBP = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["GBP"] = new JObject
                            {
                                ["price"] = 36000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForAUD = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["AUD"] = new JObject
                            {
                                ["price"] = 65000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };

            var expectedQuotations = new List<CryptoCurrencyQuotation>
            {
                new CryptoCurrencyQuotation { Currency = "USD", Price = 50000.0 },
                new CryptoCurrencyQuotation { Currency = "EUR", Price = 42000.0 },
                new CryptoCurrencyQuotation { Currency = "BRL", Price = 270000.0 },
                new CryptoCurrencyQuotation { Currency = "GBP", Price = 36000.0 },
                new CryptoCurrencyQuotation { Currency = "AUD", Price = 65000.0 }
            };


            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "USD"))).ReturnsAsync(restClientResultForUSD);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "EUR"))).ReturnsAsync(restClientResultForEUR);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "BRL"))).ReturnsAsync(restClientResultForBRL);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "GBP"))).ReturnsAsync(restClientResultForGBP);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "AUD"))).ReturnsAsync(restClientResultForAUD);



            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEquivalentTo(expectedQuotations);
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));
            _mockLogger.Verify(x => x.Log(
    LogLevel.Warning,
     It.IsAny<EventId>(),
     It.Is<It.IsAnyType>((v, t) => v.ToString() == warningMessage),
     It.IsAny<Exception>(),
     (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);
        }


        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnEmptyResult_WhenRestClientReturnsNull(string currencyCode)
        {
            // Arrange
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>())).ReturnsAsync((CryptoCurrencyQuotationApiResponse)null);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            VerifyLoggerWarningWhenNoData("cryptoQuotationResult - GetQuoteForCryptoAsync returns no data for currency");

        }


        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnEmptyResult_WhenRestClientReturnsDataNull(string currencyCode)
        {
            // Arrange

            var restClientResult = new CryptoCurrencyQuotationApiResponse
            {
                Data = null
            };
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>())).ReturnsAsync(restClientResult);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            VerifyLoggerWarningWhenNoData("cryptoQuotationResult - GetQuoteForCryptoAsync returns no data for currency");
        }

        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnEmptyResult_WhenRestClientReturnsDataCurrencyCodeNull(string currencyCode)
        {
            // Arrange

            var restClientResult = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject { }
            };
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>())).ReturnsAsync(restClientResult);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            VerifyLoggerWarningWhenNoData("cryptoData has no data for currency");
        }

        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnEmptyResult_WhenRestClientReturnsDataCurrencyCodeQuoteNull(string currencyCode)
        {
            // Arrange

            var restClientResult = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject { }

                }
            };
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>())).ReturnsAsync(restClientResult);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            VerifyLoggerWarningWhenNoData("quote has no data for currency");
        }

        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnEmptyResult_WhenRestClientReturnsDataCurrencyCodeQuotationDetailsNull(string currencyCode)
        {
            // Arrange

            var restClientResult = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject { }
                    }
                }
            };
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>())).ReturnsAsync(restClientResult);

            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEmpty();
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));

            VerifyLoggerWarningWhenNoData("quotationDetails has no data for currency");
        }


        [Theory]
        [InlineData("BTC")]
        public async Task GetCryptoQuotationAsync_ShouldReturnEmptyResult_WhenRestClientReturnsInvalidData(string currencyCode)
        {
            // Arrange
            string warningMessage = $"cryptoQuotationResult - GetQuoteForCryptoAsync returns no data.";
            var restClientResultForUSD = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["USD"] = new JObject
                            {
                                ["price"] = "invalid",
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForEUR = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["EUR"] = new JObject
                            {
                                ["price"] = null,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForBRL = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["BRL"] = new JObject
                            {
                                ["price"] = 270000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForGBP = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["GBP"] = new JObject
                            {
                                ["price"] = 36000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };
            var restClientResultForAUD = new CryptoCurrencyQuotationApiResponse
            {
                Data = new JObject
                {
                    [currencyCode] = new JObject
                    {
                        ["quote"] = new JObject
                        {
                            ["AUD"] = new JObject
                            {
                                ["price"] = 65000.0,
                                ["lastUpdated"] = DateTime.Now
                            }

                        }
                    }
                }
            };

            var expectedQuotations = new List<CryptoCurrencyQuotation>
            {
                new CryptoCurrencyQuotation { Currency = "USD", Price = 0 },
                new CryptoCurrencyQuotation { Currency = "EUR", Price = 0 },
                new CryptoCurrencyQuotation { Currency = "BRL", Price = 270000.0 },
                new CryptoCurrencyQuotation { Currency = "GBP", Price = 36000.0 },
                new CryptoCurrencyQuotation { Currency = "AUD", Price = 65000.0 }
            };


            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "USD"))).ReturnsAsync(restClientResultForUSD);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "EUR"))).ReturnsAsync(restClientResultForEUR);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "BRL"))).ReturnsAsync(restClientResultForBRL);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "GBP"))).ReturnsAsync(restClientResultForGBP);
            _mockRestClient.Setup(x => x.GetQuoteForCryptoAsync(currencyCode, It.Is<string>(s => s == "AUD"))).ReturnsAsync(restClientResultForAUD);



            // Act
            var result = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencyCode);

            // Assert
            result.Should().BeEquivalentTo(expectedQuotations);
            _mockRestClient.Verify(x => x.GetQuoteForCryptoAsync(currencyCode, It.IsAny<string>()), Times.Exactly(5));
            _mockLogger.Verify(x => x.Log(
    LogLevel.Warning,
     It.IsAny<EventId>(),
     It.Is<It.IsAnyType>((v, t) => v.ToString() == warningMessage),
     It.IsAny<Exception>(),
     (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);
        }


        #endregion

        #region Private Method
        private void VerifyLoggerWarningWhenNoData(string initialMessage)
        {
            List<string> currencies = new List<string>() { "USD", "EUR", "BRL", "GBP", "AUD" };
            foreach (var currency in currencies)
            {
                var warningMessage = $"{initialMessage} {currency}";
                _mockLogger.Verify(x => x.Log(
    LogLevel.Warning,
     It.IsAny<EventId>(),
     It.Is<It.IsAnyType>((v, t) => v.ToString() == warningMessage),
     It.IsAny<Exception>(),
     (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
            }
        }
        #endregion
    }
}
