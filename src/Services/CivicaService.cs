using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Services.Models;
using civica_service.Utils.StorageProvider;
using civica_service.Utils.Xml;
using Newtonsoft.Json;
using StockportGovUK.AspNetCore.Gateways;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;
using CouncilTaxAccountResponse = StockportGovUK.NetStandard.Models.Civica.CouncilTax.CouncilTaxAccountResponse;
using Instalment = StockportGovUK.NetStandard.Models.Civica.CouncilTax.Instalment;

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
        // nonsence of assigning a council-tax-reference to a session-id when this endpoint is called.
        private async Task SetAccountRefererance(string personReference, string accountReference)
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
            var cacheResponse = await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.Benefits}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<BenefitsClaimSummary>>(cacheResponse);
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
            var parsedResponse =
                _xmlParser.DeserializeXmlStringToType<BenefitsClaimsSummaryResponse>(content, "HBSelectDoc");
            var claimSummary = parsedResponse.Claims.Summary;

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.Benefits}",
                JsonConvert.SerializeObject(claimSummary));

            return claimSummary;
        }

        public async Task<BenefitsClaim> GetBenefitDetails(string personReference, string claimReference,
            string placeReference)
        {
            var key = $"{personReference}-{claimReference}-{placeReference}-{CacheKeys.ClaimDetails}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<BenefitsClaim>(cacheResponse);
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
            var parsedResponse =
                _xmlParser.DeserializeXmlStringToType<BenefitsClaim>(responseContent, "HBClaimDetails");

            _ = _cacheProvider.SetStringAsync(key, JsonConvert.SerializeObject(parsedResponse));

            return parsedResponse;
        }

        public async Task<List<PaymentDetail>> GetHousingBenefitPaymentHistory(string personReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.HousingBenefitsPaymentDetails}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<PaymentDetail>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "hbpaydet")
                .Add("type", "prp")
                .Build();

            var response = await _gateway.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var paymentDetails =
                _xmlParser.DeserializeXmlStringToType<PaymentDetailsResponse>(content, "HBPaymentDetails");
            var housingBenefitList = paymentDetails.PaymentList.PaymentDetails;

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.HousingBenefitsPaymentDetails}",
                JsonConvert.SerializeObject(housingBenefitList));

            return housingBenefitList;
        }

        public async Task<List<PaymentDetail>> GetCouncilTaxBenefitPaymentHistory(string personReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.CouncilTaxPaymentDetails}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<PaymentDetail>>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "hbpaydet")
                .Add("type", "ctp")
                .Build();

            var response = await _gateway.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var paymentDetails =
                _xmlParser.DeserializeXmlStringToType<PaymentDetailsResponse>(content, "HBPaymentDetails");
            var ctaxPaymentList = paymentDetails.PaymentList.PaymentDetails;

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.CouncilTaxPaymentDetails}",
                JsonConvert.SerializeObject(ctaxPaymentList));

            return ctaxPaymentList;
        }

        public async Task<List<CouncilTaxDocumentReference>> GetDocuments(string personReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.CouncilTaxDocuments}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<CouncilTaxDocumentReference>>(cacheResponse);
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

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.CouncilTaxDocuments}",
                JsonConvert.SerializeObject(documentsList));

            return documentsList;
        }

        public async Task<List<CouncilTaxDocumentReference>> GetDocumentsWithAccountReference(string personReference,
            string accountReference)
        {
            var key = $"{personReference}-{accountReference}-{CacheKeys.CouncilTaxDocumentsByReference}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<CouncilTaxDocumentReference>>(cacheResponse);
            }

            var documents = await GetDocuments(personReference);
            var filteredDocuments = documents
                .Where(_ => _.AccountReference == accountReference)
                .ToList();

            _ = _cacheProvider.SetStringAsync(key, JsonConvert.SerializeObject(filteredDocuments));

            return filteredDocuments;
        }

        public async Task<List<Place>> GetPropertiesOwned(string personReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.CouncilTaxPropertiesOwned}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<Place>>(cacheResponse);
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

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.CouncilTaxPropertiesOwned}",
                JsonConvert.SerializeObject(places));

            return places;
        }

        public async Task<Place> GetCurrentProperty(string personReference, string accountReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.CouncilTaxCurrentProperty}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<Place>(cacheResponse);
            }

            //required for civica to return correct address for user
            await SetAccountRefererance(personReference, accountReference);

            var properties = await GetPropertiesOwned(personReference);
            var currentProperty = properties.FirstOrDefault(p => p.Status == "Current") ?? properties.FirstOrDefault();

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.CouncilTaxCurrentProperty}",
                JsonConvert.SerializeObject(currentProperty));

            return currentProperty;
        }

        public async Task<List<CtaxActDetails>> GetAccounts(string personReference)
        {
            var cacheResponse =
                await _cacheProvider.GetStringAsync($"{personReference}-{CacheKeys.CouncilTaxAccounts}");

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<CtaxActDetails>>(cacheResponse);
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

            _ = _cacheProvider.SetStringAsync($"{personReference}-{CacheKeys.CouncilTaxAccounts}",
                JsonConvert.SerializeObject(accounts));

            return accounts;
        }

        public async Task<List<Instalment>> GetPaymentSchedule(string personReference, int year)
        {
            var key = $"{personReference}-{year}-{CacheKeys.CouncilTaxPaymentSchedule}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<Instalment>>(cacheResponse);
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
                .DeserializeDescendentsToIEnumerable<Instalment>(responseContent, "Instalment")
                .ToList();

            _ = _cacheProvider.SetStringAsync(key, JsonConvert.SerializeObject(parsedResponse));

            return parsedResponse;
        }

        public async Task<CouncilTaxAccountResponse> GetCouncilTaxDetails(string personReference,
            string accountReference)
        {
            var key = $"{personReference}-{accountReference}-{CacheKeys.CouncilTaxAccount}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<CouncilTaxAccountResponse>(cacheResponse);
            }

            var sessionId = await _sessionProvider.GetSessionId(personReference);

            var url = _queryBuilder
                .Add("sessionId", sessionId)
                .Add("docid", "ctxdet")
                .Add("actref", accountReference)
                .Build();

            var response = await _gateway.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var parsedResponse =
                _xmlParser.DeserializeXmlStringToType<CouncilTaxAccountResponse>(responseContent, "CtaxDetails");

            _ = _cacheProvider.SetStringAsync(key, JsonConvert.SerializeObject(parsedResponse));

            return parsedResponse;
        }

        public async Task<RecievedYearTotal> GetCouncilTaxDetailsForYear(string personReference,
            string accountReference, string year)
        {
            var key = $"{personReference}-{accountReference}-{year}-{CacheKeys.CouncilTaxAccountForYear}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<RecievedYearTotal>(cacheResponse);
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
            var parsedResponse =
                _xmlParser.DeserializeXmlStringToType<CouncilTaxAccountSummary>(responseContent, "CtaxDetails");
            var receivedYearTotals = parsedResponse.FinancialDetails.RecievedYearTotal;

            _ = _cacheProvider.SetStringAsync(key, JsonConvert.SerializeObject(receivedYearTotals));

            return receivedYearTotals;
        }

        public async Task<List<Transaction>> GetAllTransactionsForYear(string personReference, int year)
        {
            var key = $"{personReference}-{year}-{CacheKeys.Transactions}";
            var cacheResponse = await _cacheProvider.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonConvert.DeserializeObject<List<Transaction>>(cacheResponse);
            }

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

            _ = _cacheProvider.SetStringAsync(key, JsonConvert.SerializeObject(transactions));

            return transactions;
        }
    }
}