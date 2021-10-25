using Challenge.CacheServer.Api.Models;
using Challenge.CacheServer.Infrastructure.Models;
using Challenge.CacheServer.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Challenge.CacheServer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> _logger;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ApiKeyStore _apiKeyStore;

        public CurrenciesController(ILogger<CurrenciesController> logger, IExchangeRateService exchangeRateService, ApiKeyStore apiKeyStore)
        {
            _logger = logger;
            _exchangeRateService = exchangeRateService;
            _apiKeyStore = apiKeyStore;
        }

        [HttpGet("exchange-rates")]
        public async Task<IActionResult> GetExchangeRates( [FromQuery] Dictionary<string, string> currencies, DateTime startPeriod, DateTime endPeriod, string apiKey)
        {
            if ( apiKey != _apiKeyStore.ApiKey || string.IsNullOrEmpty(apiKey) )
            {
                _logger.LogWarning($"Unauthorized access attempted with api key: {apiKey}");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            if (startPeriod > DateTime.Now || endPeriod > DateTime.Now)
                return StatusCode(StatusCodes.Status404NotFound);
            if (startPeriod > endPeriod)
                return StatusCode(StatusCodes.Status400BadRequest);

            try
            {
                return Ok(await _exchangeRateService.GetAsync(currencies, startPeriod, endPeriod));
            }
            catch(EcbServerMessageException e)
            {
                return StatusCode((int)e.ServerStatusCode, e.Message);
            }
            catch(Exception e)
            {
                _logger.LogError($"Exception occured when serving request: {e.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
