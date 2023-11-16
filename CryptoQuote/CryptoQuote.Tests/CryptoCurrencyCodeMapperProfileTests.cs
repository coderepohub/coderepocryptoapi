using AutoMapper;
using CryptoQuote.Agent.Mappers;
using CryptoQuote.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoQuote.Tests
{
    public class CryptoCurrencyCodeMapperProfileTests
    {
        [Fact]
        public void Mapping_Configuration_Is_Valid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CryptoCurrencyCodeMapperProfile>();
            });

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_CryptoCurrency_To_CryptoCurrencyCodeResponse_Is_Valid()
        {
            // Arrange
            var cryptoCurrency = new CryptoCurrency
            {
                Symbol = "BTC",
                Name = "Bitcoin"
            };

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CryptoCurrencyCodeMapperProfile>();
            });

            var mapper = configuration.CreateMapper();

            // Act
            var result = mapper.Map<CryptoCurrencyCodeResponse>(cryptoCurrency);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be(cryptoCurrency.Symbol);
            result.Name.Should().Be(cryptoCurrency.Name);
        }
    }
}
