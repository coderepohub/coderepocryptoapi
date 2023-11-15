using CryptoQuote.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CryptoQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoQuoteApiController : ControllerBase
    {
        private readonly ICryptoQuoteAgent _cryptoQuoteAgent;
        private readonly ILogger<CryptoQuoteApiController> _logger;
        public CryptoQuoteApiController(ICryptoQuoteAgent cryptoQuoteAgent, ILogger<CryptoQuoteApiController> logger)
        {
            _cryptoQuoteAgent = cryptoQuoteAgent ?? throw new ArgumentNullException(nameof(cryptoQuoteAgent));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        /// <summary>
        /// Returns list of Crypto Currency Codes.
        /// </summary>
        /// <param name="limit">Pass the limit of data (optional) by default it is set to 50.</param>
        /// <returns>Returns list of crypto currency codes.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int limit = 50)
        {
            _logger.Log(LogLevel.Information, $"Get Crypto currency code api endpoint called.");
            var cryptoCurrencyCode = await _cryptoQuoteAgent.GetCryptoCurrencyCodesAsync(limit);
            if (cryptoCurrencyCode is null || !cryptoCurrencyCode.Any())
            {
                _logger.Log(LogLevel.Error, $"Get Crypto currency code api endpoint returned no result.");
                return BadRequest();
            }
            return Ok(cryptoCurrencyCode);
        }


        /// <summary>
        /// Get the list of crypto currency exchange rate for USD, EUR, BRL, GBP, AUD
        /// </summary>
        /// <param name="currencycode">Currency code as BTC, etc.</param>
        /// <returns>Returns list of crypto currency exhange rates data</returns>
        [HttpGet("/quotation/{currencycode}")]
        public async Task<IActionResult> Get(string currencycode)
        {
            _logger.Log(LogLevel.Information, $"Get Crypto currency quotation api endpoint called.");
            var cryptoQuotation = await _cryptoQuoteAgent.GetCryptoQuotationAsync(currencycode);
            if (cryptoQuotation is null || !cryptoQuotation.Any())
            {
                _logger.Log(LogLevel.Error, $"Get Crypto currency quotation api endpoint returned no result.");
                return BadRequest();
            }

            return Ok(cryptoQuotation);
        }
    }
}
