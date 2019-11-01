using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Utils.Xml;
using civica_service.Services.Models;
using StockportGovUK.AspNetCore.Gateways;
using System.Linq;

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
    }
}
