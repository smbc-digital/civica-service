using civica_service.Helpers.SessionProvider;
using StockportGovUK.AspNetCore.Gateways.RevsBensServiceGateway;

namespace civica_service.Services
{
    public class CouncilTaxService
    {
        private readonly IRevsBensServiceGateway _revsBensServiceGateway;
        private readonly ICivicaService _civicaService;
        private readonly ISessionProvider _sessionProvider;
        public CouncilTaxService(IRevsBensServiceGateway revsBensServiceGateway, ICivicaService civicaService, ISessionProvider sessionProvider)
        {
            _revsBensServiceGateway = revsBensServiceGateway;
            _civicaService = civicaService;
            _sessionProvider = sessionProvider;
        }

        public string GetCouncilTaxTransactions(string personReference, string accountReference, int year)
        {
            var sessionId =  _sessionProvider.GetSessionId(personReference);
            _civicaService.GetAllTransactionsForYear(sessionId, year);
        }
    }
}

