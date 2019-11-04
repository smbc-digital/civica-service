using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Utils.Xml;
using civica_service.Services.Models;
using StockportGovUK.AspNetCore.Gateways;
using System.Linq;
using System.Collections.Generic;
using civica_service.Helpers.SessionProvider.Models;

namespace civica_service.Services
{
    public class CivicaService : ICivicaService
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ISessionProvider _sessionProvider;

        public CivicaService(IGateway gateway, IQueryBuilder queryBuilder, ISessionProvider sessionProvider)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _sessionProvider = sessionProvider;
        }

        public async Task<bool> IsBenefitsClaimant(string personReference)
        {
            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "hbsel")
                .Add("sessionId", sessionId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var xmlResponse = await response.Content.ReadAsStringAsync();
            var claimsSummaryResponse = XmlParser.DeserializeXmlStringToType<ClaimsSummaryResponse>(xmlResponse, "HBSelectDoc");

            return claimsSummaryResponse.ClaimsList != null && claimsSummaryResponse.ClaimsList.ClaimSummary.Any();
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

