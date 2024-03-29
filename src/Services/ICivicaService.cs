﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services {
    public interface ICivicaService {
        Task<string> GetSessionId (string personReference);

        Task<bool> IsBenefitsClaimant (string personReference);

        Task<CouncilTaxAccountResponse> GetCouncilTaxDetails (string personReference, string accountReference);

        Task<ReceivedYearTotal> GetCouncilTaxDetailsForYear (string personReference, string accountReference, string year);

        Task<List<Transaction>> GetAllTransactionsForYear (string personReference, string accountReference, int year);

        Task<List<BenefitsClaimSummary>> GetBenefits (string personReference);

        Task<BenefitsClaim> GetBenefitDetails (string personReference, string claimReference, string placeReference);

        Task<List<PaymentDetail>> GetHousingBenefitPaymentHistory (string personReference);

        Task<List<PaymentDetail>> GetCouncilTaxBenefitPaymentHistory (string personReference);

        Task<List<Installment>> GetPaymentSchedule(string personReference, int year);

        Task<List<CouncilTaxDocumentReference>> GetDocuments (string personReference);

        Task<List<CouncilTaxDocumentReference>> GetDocumentsWithAccountReference (string personReference, string accountReference);

        Task<List<Place>> GetPropertiesOwned (string personReference, string accountReference = "");

        Task<Place> GetCurrentProperty (string personReference, string accountReference);

        Task<List<CtaxActDetails>> GetAccounts (string personReference);

        Task<StockportGovUK.NetStandard.Models.Civica.CouncilTax.PersonName> GetPerson(string personReference);

        Task<byte[]> GetDocumentForAccount(string personReference, string accountReference, string documentId);
    }
}