using System.Threading.Tasks;
using civica_service.Services.Models;

namespace civica_service.Services
{
    public interface ICivicaService
    {
        Task<bool> IsBenefitsClaimant(string personReference);

        Task<ClaimsSummaryResponse> GetBenefits(string personReference);
    }
}
