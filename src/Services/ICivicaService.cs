using System.Threading.Tasks;

namespace civica_service.Services
{
    public interface ICivicaService
    {
        Task<bool> IsBenefitsClaimant(string personReference);
    }
}
