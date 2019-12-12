using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockportGovUK.AspNetCore.Gateways;
using civica_service.Utils.StorageProvider;

namespace civica_service.Helpers.SessionProvider
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly SessionConfiguration _configuration;
        private readonly ICacheProvider _distributedCache;
        private readonly IXmlParser _xmlParser;

        private readonly ILogger<SessionProvider> _logger;

        public SessionProvider(
            IGateway gateway, 
            IQueryBuilder queryBuilder, 
            IOptions<SessionConfiguration> configuration,
            ICacheProvider distributedCache,
            IXmlParser xmlParser,
            ILogger<SessionProvider> logger)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _configuration = configuration.Value;
            _distributedCache = distributedCache;
            _xmlParser = xmlParser;
            _logger = logger;
        }

        public async Task<string> GetSessionId()
        {
            var url = _queryBuilder
                .Add("docid", "crmlogin")
                .Add("userid", _configuration.Username)
                .Add("password", _configuration.Password)
                .Build();

            var response = await _gateway.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Civica is unavailable. Responded with status code: {response.StatusCode}");
            }

            var xmlResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"GetSessionId - url: {url}, xmlResponse: {xmlResponse}");

            var deserializedResponse = _xmlParser.DeserializeXmlStringToType<SessionIdModel>(xmlResponse, "Login").Result;

            if (!deserializedResponse.ErrorCode.Text.Equals("5"))
            {
                throw new Exception($"API login unsuccessful, check credentials. Actual response: {xmlResponse.ToString()}");
            }

            var sessionId = deserializedResponse.SessionID;

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new Exception("No session id returned");
            }

            return sessionId;
        }

        public async Task<string> GetSessionId(string personReference)
        {
            var cacheResponse = await _distributedCache.GetStringAsync($"{personReference}-{ECacheKeys.SessionId}");
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return cacheResponse;
            }

            var sessionId = await GetSessionId();

            if (!await AssignPersonToSession(sessionId, personReference))
            {
                throw new Exception($"Could not assign person reference {personReference} to session {sessionId}");
            }

            _ = _distributedCache.SetStringAsync($"{personReference}-{ECacheKeys.SessionId}", sessionId);

            return sessionId;
        }

        private async Task<bool> AssignPersonToSession(string sessionId, string personReference)
        {
            var url = _queryBuilder
                .Add("SessionId", sessionId)
                .Add("docid", "cocrmper")
                .Add("personref", personReference)
                .Build();

            var response = await _gateway.GetAsync(url);
            var xmlResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"AssignPersonToSession - url: {url}, xmlResponse: {xmlResponse}");

            var result = _xmlParser.DeserializeXmlStringToType<SetPersonModel>(xmlResponse, "SetPerson");

            var matchingErrorCodes = new List<string>
            {
                "Person Reference Set",
                "Person CouncilTaxReference Set"
            };

            return matchingErrorCodes.Contains(result.ErrorCode.Description);
        }
    }
}
