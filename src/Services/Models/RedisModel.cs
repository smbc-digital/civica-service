namespace civica_service.Services.Models
{
    public class CacheModel
    {
        public string SessionId { get; set; }

        public ClaimsSummaryResponse ClaimsSummary { get; set; }
    }
}
