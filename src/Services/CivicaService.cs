using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Utils.Xml;
using civica_service.Services.Models;
using StockportGovUK.AspNetCore.Gateways;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using civica_service.Utils.StorageProvider;
using System.Collections.Generic;
using System.Net.Http;

namespace civica_service.Services
{
    public class CivicaService : ICivicaService
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ISessionProvider _sessionProvider;
        private readonly IDistributedCache _cacheProvider;

        public CivicaService(IGateway gateway, IQueryBuilder queryBuilder, ISessionProvider sessionProvider, IDistributedCache cacheProvider)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _sessionProvider = sessionProvider;
            _cacheProvider = cacheProvider;
        }

        public async Task<bool> IsBenefitsClaimant(string personReference)
        {
            var claimsSummaryResponse = await GetBenefits(personReference);

            return claimsSummaryResponse.Claims != null && claimsSummaryResponse.Claims.Summary.Any();
        }

        public async Task<ClaimsSummaryResponse> GetBenefits(string personReference)
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.Benefits}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<ClaimsSummaryResponse>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "hbsel")
                .Add("sessionId", sessionId)
                .Build();

            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("claimsts", "All")
            });

            var response = await _gateway.PostAsync(url, content);
            var reponseContent = await response.Content.ReadAsStringAsync();
            var claimSummary = XmlParser.DeserializeXmlStringToType<ClaimsSummaryResponse>(reponseContent, "HBSelectDoc");

            _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.Benefits}", JsonConvert.SerializeObject(claimSummary));
            
            return claimSummary;
        }
    }
}
