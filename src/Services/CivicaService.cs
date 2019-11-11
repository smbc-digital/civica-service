using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Utils.Xml;
using civica_service.Services.Models;
using StockportGovUK.AspNetCore.Gateways;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

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

            return claimsSummaryResponse.ClaimsList != null && claimsSummaryResponse.ClaimsList.ClaimSummary.Any();
        }

        public async Task<ClaimsSummaryResponse> GetBenefits(string personReference)
        {
            var cacheModel = new CacheModel();
            var cacheResponse = await _cacheProvider.GetStringAsync(personReference);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                cacheModel = JsonConvert.DeserializeObject<CacheModel>(cacheResponse);

                if (cacheModel.ClaimsSummary != null)
                {
                    return cacheModel.ClaimsSummary;
                }
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "hbsel")
                .Add("sessionId", sessionId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var claimSummary = XmlParser.DeserializeXmlStringToType<ClaimsSummaryResponse>(content, "HBSelectDoc");

            // TODO: get value from redis again and update our values
            cacheModel.ClaimsSummary = claimSummary;
            _cacheProvider.SetStringAsync(personReference, JsonConvert.SerializeObject(cacheModel));

            return claimSummary;
        }
    }
}
