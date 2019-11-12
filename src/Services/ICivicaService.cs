using civica_service.Helpers.SessionProvider.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using civica_service.Services.Models;

namespace civica_service.Services
{
    public interface ICivicaService
    {
        Task<bool> IsBenefitsClaimant(string personReference);

        Task<IEnumerable<TransactionModel>> GetAllTransactionsForYear(string personReference, int year);

        Task<ClaimsSummaryResponse> GetBenefits(string personReference);
    }
}
