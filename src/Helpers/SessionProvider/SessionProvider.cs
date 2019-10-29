using System;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider.Models;
using Microsoft.Extensions.Options;
using StockportGovUK.AspNetCore.Gateways;

namespace civica_service.Helpers.SessionProvider
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly SessionConfiguration _configuration;

        public SessionProvider(
            IGateway gateway, 
            IQueryBuilder queryBuilder, 
            IOptions<SessionConfiguration> configuration)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _configuration = configuration.Value;
        }

        public async Task<string> GetSessionId(string personReference)
        {
            /**
             * Go to redis for key -- return if real boy
             * generate new key
             * register person ref against session 
             * store in redis
             * return sessionId
             */

            var url = _queryBuilder
                .Add("docid", "crmlogin")
                .Add("userid", _configuration.Username)
                .Add("password", _configuration.Password)
                .Build();

            throw new NotImplementedException();
        }
    }
}
