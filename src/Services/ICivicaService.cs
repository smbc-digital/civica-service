using civica_service.Helpers.SessionProvider.Models;
using System.Threading.Tasks;

namespace civica_service.Services
{
    public interface ICivicaService
    {
        Task<bool> IsBenefitsClaimant(string personReference);

        void GetCouncilTaxDetails(string personReference, string accountReference);

        Task<TransactionListModel> GetAllTransactionsForYear(string personReference, int year);
    }
}
