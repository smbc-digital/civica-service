using System.Threading.Tasks;

namespace civica_service.Helpers.SessionProvider
{
    public interface ISessionProvider
    {
        Task<string> GetSessionId(string personReference);
    }
}
