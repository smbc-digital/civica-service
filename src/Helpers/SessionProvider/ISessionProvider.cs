using System.Threading.Tasks;

namespace civica_service.Helpers.SessionProvider
{
    public interface ISessionProvider
    {
        Task<string> GetAnonymousSessionId();
        Task<string> GetSessionId();
        Task<string> GetSessionId(string personReference);
    }
}
