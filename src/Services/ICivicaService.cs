using System.Collections.Generic;
using System.Threading.Tasks;
using civica_service.Services.Models;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services {
    public interface ICivicaService {
        Task<string> GetSessionId (string personReference);

        Task<bool> IsBenefitsClaimant (string personReference);

        Task<CouncilTaxAccountResponse> GetCouncilTaxDetails (string personReference, string accountReference);

        Task<RecievedYearTotal> GetCouncilTaxDetailsForYear (string personReference, string accountReference, string year);

        Task<List<Transaction>> GetAllTransactionsForYear (string personReference, int year);

        Task<List<BenefitsClaimSummary>> GetBenefits (string personReference);

        Task<BenefitsClaim> GetBenefitDetails (string personReference, string claimReference, string placeReference);

        Task<List<PaymentDetail>> GetHousingBenefitPaymentHistory (string personReference);

        Task<List<PaymentDetail>> GetCouncilTaxBenefitPaymentHistory (string personReference);

        Task<CouncilTaxPaymentScheduleResponse> GetPaymentSchedule(string personReference, int year);

        Task<List<CouncilTaxDocument>> GetDocuments (string personReference);

        Task<List<CouncilTaxDocument>> GetDocumentsWithAccountReference (string personReference, string accountReference);

        Task<List<Place>> GetPropertiesOwned (string personReference);

        Task<Place> GetCurrentProperty (string personReference);

        Task<List<CouncilTaxAccountDetails>> GetAccounts (string personReference);
    }
}