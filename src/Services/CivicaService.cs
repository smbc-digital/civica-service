using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Utils.Xml;
using civica_service.Services.Models;
using StockportGovUK.AspNetCore.Gateways;
using System.Linq;
using System.Collections.Generic;
using civica_service.Helpers.SessionProvider.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using civica_service.Utils.StorageProvider;
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

        // TODO: this is a useful call that may be needed later
        // Currently we don't need the info from it, hence the void.
        public void GetCouncilTaxDetails(string personReference, string accountReference)
        {
            var sessionId = _sessionProvider.GetSessionId(personReference).Result;

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "ctxdet")
                .Add("actref", accountReference)
                .Build();

            _gateway.GetAsync(url);
        }

        public async Task<TransactionListModel> GetAllTransactionsForYear(string personReference, int year)
        {
            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "ctxtrn")
                .Add("recyear", year.ToString())
                .Add("trantype", "All")
                .Build();

            var response = await _gateway.GetAsync(url);
            var xmlResponse = await response.Content.ReadAsStringAsync();

            return XmlParser.DeserializeXmlStringToType<TransactionListModel>(xmlResponse, "TranList");
        }
    }
}

