using System;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using StockportGovUK.AspNetCore.Gateways;

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
            /**
             * Add session -- done
             * store session in redis -- done
             * Build query -- done
             * Call endpoint 
             */
            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("", "")
                .Add("", "")
                .Add("sessionId", sessionId)
                .Build();

            throw new NotImplementedException("loser");
        }
    }
}
