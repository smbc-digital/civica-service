using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.Xml;
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
            // TODO: Check storage provider for key and return if found

            var url = _queryBuilder
                .Add("outputtype", "xml")
                .Add("docid", "crmlogin")
                .Add("userid", _configuration.Username)
                .Add("password", _configuration.Password)
                .Build();

            var response = await _gateway.GetAsync(url);
            var xmlResponse = await response.Content.ReadAsStringAsync();

            var deserializedResponse = XmlParser.DeserializeXmlStringToType<SessionIdModel>(xmlResponse, "Login").Result;

            if (!deserializedResponse.ErrorCode.Text.Equals(5))
            {
                throw new Exception($"API login unsuccessful, check credentials. Actual response: {xmlResponse.ToString()}");
            }

            if (string.IsNullOrWhiteSpace(deserializedResponse.SessionID))
            {
                throw new Exception("No session id returned");
            }

            if (!await AssignPersonToSession(deserializedResponse.SessionID, personReference))
            {
                throw new Exception($"Could not assign person reference {personReference} to session {deserializedResponse.SessionID}");
            }

            // TODO: Save sessionId to storage provider

            return deserializedResponse.SessionID;
        }

        private async Task<bool> AssignPersonToSession(string sessionId, string personReference)
        {
            var url = _queryBuilder
                .Add("outputtype", "xml")
                .Add("SessionId", sessionId)
                .Add("docid", "cocrmper")
                .Add("personref", personReference)
                .Build();

            var resposne = await _gateway.GetAsync(url);
            var xmlResponse = await resposne.Content.ReadAsStringAsync();

            var result = XmlParser.DeserializeXmlStringToType<SetPersonModel>(xmlResponse, "ErrorCode");

            var matchingErrorCodes = new List<string>
            {
                "Person Reference Set",
                "Person CouncilTaxReference Set"
            };

            return matchingErrorCodes.Contains(result.ErrorCode.Description);
        }
    }
}
