using System.Collections.Generic;
using System.Threading.Tasks;
using civica_service.Helpers.SessionProvider.Models;
using StockportGovUK.NetStandard.Models.Models.Civica.CouncilTax;

namespace civica_service.Services
{
    public interface ICivicaService
    {
        Task<bool> IsBenefitsClaimant(string personReference);

        Task<CouncilTaxAccountResponse> GetCouncilTaxDetails(string personReference, string accountReference);

        Task<TransactionListModel> GetAllTransactionsForYear(string personReference, int year);

        Task<ClaimsSummaryResponse> GetBenefits(string personReference);

        Task<List<CouncilTaxDocumentReference>> GetDocuments(string personReference);

        Task<List<CouncilTaxDocumentReference>> GetDocumentsWithAccountReference(string personReference, string accountReference);

        Task<List<Places>> GetPropertiesOwned(string personReference);

        Task<Places> GetCurrentProperty(string personReference);

        Task<IEnumerable<CtaxActDetails>> GetAccounts(string personReference);

        Task<CouncilTaxPaymentScheduleResponse> GetPaymentSchedule(string personReference, string year);
    }
}
