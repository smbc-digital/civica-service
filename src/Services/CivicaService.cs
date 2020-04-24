using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Services.Models;
using civica_service.Utils.StorageProvider;
using civica_service.Utils.Xml;
using StockportGovUK.NetStandard.Gateways;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services
{
    public class CivicaService : ICivicaService
    {
        private readonly IGateway _gateway;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ISessionProvider _sessionProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly IXmlParser _xmlParser;

        public CivicaService(IGateway gateway, IQueryBuilder queryBuilder, ISessionProvider sessionProvider,
            ICacheProvider cacheProvider, IXmlParser xmlParser)
        {
            _gateway = gateway;
            _queryBuilder = queryBuilder;
            _sessionProvider = sessionProvider;
            _cacheProvider = cacheProvider;
            _xmlParser = xmlParser;
        }

        public async Task<string> GetSessionId(string personReference)
        {
            return await _sessionProvider.GetSessionId(personReference);
        }

        // This method may seem pointless, however, I am trying to wrap Civica's 
        // nonsense of assigning a council-tax-reference to a session-id when this endpoint is called.
        private async Task SetAccountReference(string personReference, string accountReference)
        {
            await GetCouncilTaxDetails(personReference, accountReference);
        }

        public async Task<bool> IsBenefitsClaimant(string personReference)
        {
            var claims = await GetBenefits(personReference);

            return claims.Any();
        }

        public async Task<List<BenefitsClaimSummary>> GetBenefits(string personReference)
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{ECacheKeys.Benefits}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<BenefitsClaimSummary>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "hbsel")
                .Add("sessionId", sessionId)
                .Build();

            var body = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("claimsts", "All")
            });

            var response = await _gateway.PostAsync(url, body);
            var content = await response.Content.ReadAsStringAsync();
            var parsedResponse = _xmlParser.DeserializeXmlStringToType<BenefitsClaimsSummaryResponse>(content, "HBSelectDoc");
            var claimSummary = parsedResponse.Claims.Summary;
            claimSummary.ForEach(_ => _.PersonName = parsedResponse.PersonName);

            _ = _cacheProvider.SetStringAsync($"{personReference}-{ECacheKeys.Benefits}", JsonSerializer.Serialize(claimSummary));

            return claimSummary;
        }

        public async Task<BenefitsClaim> GetBenefitDetails(string personReference, string claimReference, string placeReference)
        {
            var key = $"{personReference}-{claimReference}-{placeReference}-{ECacheKeys.ClaimDetails}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<BenefitsClaim>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "hbdet")
                .Add("claimref", claimReference)
                .Add("placeref", placeReference)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            BenefitsClaim parsedResponse;

            try
            {
                parsedResponse = _xmlParser.DeserializeXmlStringToType<BenefitsClaim>(responseContent, "HBClaimDetails");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to deserialize XML - Person reference: {personReference}, Claim reference: {claimReference}, Place reference {placeReference}, Response: {responseContent}", ex.InnerException);
            }

            _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(parsedResponse));

            return parsedResponse;
        }

        public async Task<List<PaymentDetail>> GetHousingBenefitPaymentHistory(string personReference)
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{ECacheKeys.HousingBenefitsPaymentDetails}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<PaymentDetail>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "hbpaydet")
                .Add("type", "prp")
                .Build();

            var response = await _gateway.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var paymentDetails = _xmlParser.DeserializeXmlStringToType<PaymentDetailsResponse>(content, "HBPaymentDetails");
            var housingBenefitList = paymentDetails.PaymentList.PaymentDetails;

            _ = _cacheProvider.SetStringAsync($"{personReference}-{ECacheKeys.HousingBenefitsPaymentDetails}", JsonSerializer.Serialize(housingBenefitList));

            return housingBenefitList;
        }

        public async Task<List<PaymentDetail>> GetCouncilTaxBenefitPaymentHistory(string personReference)
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{ECacheKeys.CouncilTaxPaymentDetails}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<PaymentDetail>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "hbpaydet")
                .Add("type", "ctp")
                .Build();

            var response = await _gateway.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var paymentDetails = _xmlParser.DeserializeXmlStringToType<PaymentDetailsResponse>(content, "HBPaymentDetails");
            var ctaxPaymentList = paymentDetails.PaymentList.PaymentDetails;

            _ = _cacheProvider.SetStringAsync($"{personReference}-{ECacheKeys.CouncilTaxPaymentDetails}", JsonSerializer.Serialize(ctaxPaymentList));

            return ctaxPaymentList;
        }

        public async Task<List<CouncilTaxDocumentReference>> GetDocuments(string personReference)
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{ECacheKeys.CouncilTaxDocuments}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<CouncilTaxDocumentReference>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "viewdoc")
                .Add("type", "all")
                .Add("sessionId", sessionId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var documentsList = _xmlParser
                .DeserializeDescendentsToIEnumerable<CouncilTaxDocumentReference>(responseContent, "Document")
                .ToList();

            _ = _cacheProvider.SetStringAsync($"{personReference}-{ECacheKeys.CouncilTaxDocuments}", JsonSerializer.Serialize(documentsList));

            return documentsList;
        }



        public async Task<List<CouncilTaxDocumentReference>> GetDocumentsWithAccountReference(string personReference,
            string accountReference)
        {
            var key = $"{personReference}-{accountReference}-{ECacheKeys.CouncilTaxDocumentsByReference}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<CouncilTaxDocumentReference>>(cacheResponse);
            }

            var documents = await GetDocuments(personReference);
            var filteredDocuments = documents
                .Where(_ => _.AccountReference == accountReference)
                .ToList();

            _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(filteredDocuments));

            return filteredDocuments;
        }

        public async Task<List<Place>> GetPropertiesOwned(string personReference, string accountReference = "")
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{accountReference}-{ECacheKeys.CouncilTaxPropertiesOwned}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<Place>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "ctxprop")
                .Add("proplist", "y")
                .Add("sessionId", sessionId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var places = _xmlParser
                .DeserializeDescendentsToIEnumerable<Place>(responseContent, "Places")
                .ToList();

            _ = _cacheProvider.SetStringAsync($"{personReference}-{accountReference}-{ECacheKeys.CouncilTaxPropertiesOwned}", JsonSerializer.Serialize(places));

            return places;
        }

        public async Task<Place> GetCurrentProperty(string personReference, string accountReference)
        {
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{accountReference}-{ECacheKeys.CouncilTaxCurrentProperty}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<Place>(cacheResponse);
            }

            //required for civica to return correct address for user
            await SetAccountReference(personReference, accountReference);

            var properties = await GetPropertiesOwned(personReference, accountReference);
            var currentProperty = properties.FirstOrDefault(p => p.Status == "Current") ?? properties.FirstOrDefault();

            _ = _cacheProvider.SetStringAsync($"{personReference}-{accountReference}-{ECacheKeys.CouncilTaxCurrentProperty}", JsonSerializer.Serialize(currentProperty));

            return currentProperty;
        }

        public async Task<List<CtaxActDetails>> GetAccounts(string personReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{ECacheKeys.CouncilTaxAccounts}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<CtaxActDetails>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "ctxsel")
                .Add("sessionId", sessionId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var accounts = _xmlParser
                .DeserializeDescendentsToIEnumerable<CtaxActDetails>(responseContent, "CtaxActDetails")
                .ToList();

            _ = _cacheProvider.SetStringAsync($"{personReference}-{ECacheKeys.CouncilTaxAccounts}", JsonSerializer.Serialize(accounts));

            return accounts;
        }

        public async Task<List<Installment>> GetPaymentSchedule(string personReference, int year)
        {
            var key = $"{personReference}-{year}-{ECacheKeys.CouncilTaxPaymentSchedule}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<Installment>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("docid", "irins")
                .Add("recno", "1")
                .Add("module", "C")
                .Add("recyear", year.ToString())
                .Add("sessionId", sessionId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var parsedResponse = _xmlParser
                .DeserializeDescendentsToIEnumerable<Installment>(responseContent, "Instalment")
                .ToList();

            _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(parsedResponse));

            return parsedResponse;
        }

        public async Task<CouncilTaxAccountResponse> GetCouncilTaxDetails(string personReference,
            string accountReference)
        {
            var key = $"{personReference}-{accountReference}-{ECacheKeys.CouncilTaxAccount}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<CouncilTaxAccountResponse>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "ctxdet")
                .Add("actref", accountReference)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            CouncilTaxAccountResponse parsedResponse;

            try
            {
                parsedResponse = _xmlParser.DeserializeXmlStringToType<CouncilTaxAccountResponse>(responseContent, "CtaxDetails");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to deserialize XML - Person reference: {personReference}, Account reference: {accountReference}, Response: {responseContent}", ex.InnerException);
            }

            _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(parsedResponse));

            return parsedResponse;
        }

        public async Task<ReceivedYearTotal> GetCouncilTaxDetailsForYear(string personReference,
            string accountReference, string year)
        {
            var key = $"{personReference}-{accountReference}-{year}-{ECacheKeys.CouncilTaxAccountForYear}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<ReceivedYearTotal>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "ctxdet")
                .Add("actref", accountReference)
                .Add("year", year)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var parsedResponse = _xmlParser.DeserializeXmlStringToType<CouncilTaxAccountSummary>(responseContent, "CtaxDetails");
            var receivedYearTotals = parsedResponse.FinancialDetails.ReceivedYearTotal;

            _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(receivedYearTotals));

            return receivedYearTotals;
        }

        public async Task<List<Transaction>> GetAllTransactionsForYear(string personReference, string accountReference, int year)
        {
            var key = $"{personReference}-{accountReference}-{year}-{ECacheKeys.Transactions}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<List<Transaction>>(cacheResponse);
            }

            await SetAccountReference(personReference, accountReference);

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "ctxtrn")
                .Add("recyear", year.ToString())
                .Add("trantype", "All")
                .Build();

            var response = await _gateway.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var transactions = _xmlParser
                .DeserializeDescendentsToIEnumerable<Transaction>(content, "Transaction")
                .ToList();

            _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(transactions));

            return transactions;
        }

        public async Task<byte[]> GetDocumentForAccount(string personReference, string accountReference, string documentId)
        {
            var key = $"{personReference}-{accountReference}-{documentId}-{ECacheKeys.Document}";

            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<byte[]>(cacheResponse);
            }

            await SetAccountReference(personReference, accountReference);

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "docdl")
                .Add("document", documentId)
                .Build();

            var response = await _gateway.GetAsync(url);
            var document = await response.Content.ReadAsByteArrayAsync();

            if (document != null && document.Length > 0)
            {
                _ = _cacheProvider.SetStringAsync(key, JsonSerializer.Serialize(document));
            }

            return document;
        }
    }
}