using Challenge.CacheServer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Challenge.CacheServer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly ApiKeyStore _apiKeyStore;
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger, ApiKeyStore keyStore) =>
            (_apiKeyStore, _logger) = (keyStore, logger);

        [HttpGet("api-key")]
        public string RegenerateApiKey()
        {
            _apiKeyStore.ApiKey = Guid.NewGuid().ToString();
            _logger.LogInformation($"New api key generated: {_apiKeyStore.ApiKey}");
            return _apiKeyStore.ApiKey;
        }
    }
}
