using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.StorageProvider;
using civica_service.Utils.Xml;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StockportGovUK.NetStandard.Gateways;

namespace civica_service.Helpers.SessionProvider
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly SessionConfiguration _configuration;
        private readonly ICacheProvider _distributedCache;
        private readonly IXmlParser _xmlParser;

        public SessionProvider(
            IGateway gateway, 
            IQueryBuilder queryBuilder, 
            IOptions<SessionConfiguration> configuration,
            ICacheProvider distributedCache,
            IXmlParser xmlParser)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _configuration = configuration.Value;
            _distributedCache = distributedCache;
            _xmlParser = xmlParser;
        }

        public async Task<string> GetAnonymousSessionId()
        {
            string cacheKey = $"{ECacheKeys.SessionId}-AnonymousAvailability";
            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                throw new Exception($"Civica is unavailable. Cached response");
            }

            var url = _queryBuilder
                .Add("docid", "login")
                .Build();

            var response = await _gateway.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await _distributedCache.SetStringAsync(cacheKey, "Unavailable", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                throw new Exception($"Civica is unavailable. Responded with status code: {response.StatusCode}");
            }

            await _distributedCache.RemoveAsync(cacheKey);

            var xmlResponse = await response.Content.ReadAsStringAsync();

            try
            {
                var deserializedResponse =
                    _xmlParser.DeserializeXmlStringToType<AnonymousSessionIdModel>(xmlResponse, "StandardInfo");
                var sessionId = deserializedResponse.SessionID;

                return sessionId;
            }
            catch (Exception ex)
            {
                await _distributedCache.SetStringAsync(cacheKey, "Unavailable", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                throw new Exception($"Unable to parse the XML response: {xmlResponse}");
            }
        }

        public async Task<string> GetSessionId()
        {
            string cacheKey = $"{ECacheKeys.SessionId}-Availability";
            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                throw new Exception($"Civica is unavailable. Cached response");
            }

            var url = _queryBuilder
                .Add("docid", "crmlogin")
                .Add("userid", _configuration.Username)
                .Add("password", _configuration.Password)
                .Build();

            var response = await _gateway.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await _distributedCache.SetStringAsync(cacheKey, "Unavailable", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                throw new Exception($"Civica is unavailable. Responded with status code: {response.StatusCode}");
            }

            await _distributedCache.RemoveAsync(cacheKey);

            var xmlResponse = await response.Content.ReadAsStringAsync();
            var deserializedResponse = new Result();

            try
            {
                deserializedResponse = _xmlParser.DeserializeXmlStringToType<SessionIdModel>(xmlResponse, "Login").Result;
            }
            catch (Exception ex)
            {
                await _distributedCache.SetStringAsync(cacheKey, "Unavailable", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                throw new Exception($"Unable to parse the XML response: {xmlResponse}");
            }

            if (!deserializedResponse.ErrorCode.Text.Equals("5"))
            {
                await _distributedCache.SetStringAsync(cacheKey, "Unavailable", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                throw new Exception($"API login unsuccessful, check credentials. Actual response: {xmlResponse}");
            }

            var sessionId = deserializedResponse.SessionID;

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                await _distributedCache.SetStringAsync(cacheKey, "Unavailable", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

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
