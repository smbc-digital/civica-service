using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.Xml;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using StockportGovUK.AspNetCore.Gateways;
using Newtonsoft.Json;
using civica_service.Utils.StorageProvider;

namespace civica_service.Helpers.SessionProvider
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly SessionConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        public SessionProvider(
            IGateway gateway, 
            IQueryBuilder queryBuilder, 
            IOptions<SessionConfiguration> configuration,
            IDistributedCache distributedCache)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _configuration = configuration.Value;
            _distributedCache = distributedCache;
        }

        public async Task<string> GetSessionId(string personReference)
        {
            var cacheResponse = _distributedCache.GetString($"{personReference}-{CacheKeys.SessionId}");
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return cacheResponse;
            }

            var url = _queryBuilder
                .Add("docid", "crmlogin")
                .Add("userid", _configuration.Username)
                .Add("password", _configuration.Password)
                .Build();

            var response = await _gateway.GetAsync(url);
            var xmlResponse = await response.Content.ReadAsStringAsync();
            var deserializedResponse = XmlParser.DeserializeXmlStringToType<SessionIdModel>(xmlResponse, "Login").Result;

            if (!deserializedResponse.ErrorCode.Text.Equals("5"))
            {
                throw new Exception($"API login unsuccessful, check credentials. Actual response: {xmlResponse.ToString()}");
            }

            var sessionId = deserializedResponse.SessionID;

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new Exception("No session id returned");
            }

            if (!await AssignPersonToSession(sessionId, personReference))
            {
                throw new Exception($"Could not assign person reference {personReference} to session {sessionId}");
            }

            _ = _distributedCache.SetStringAsync($"{personReference}-{CacheKeys.SessionId}", sessionId);

            return sessionId;
        }

        private async Task<bool> AssignPersonToSession(string sessionId, string personReference)
        {
            var url = _queryBuilder
                .Add("SessionId", sessionId)
                .Add("docid", "cocrmper")
                .Add("personref", personReference)
                .Build();

            var resposne = await _gateway.GetAsync(url);
            var xmlResponse = await resposne.Content.ReadAsStringAsync();
            var result = XmlParser.DeserializeXmlStringToType<SetPersonModel>(xmlResponse, "SetPerson");

            var matchingErrorCodes = new List<string>
            {
                "Person Reference Set",
                "Person CouncilTaxReference Set"
            };

            return matchingErrorCodes.Contains(result.ErrorCode.Description);
        }
    }
}
