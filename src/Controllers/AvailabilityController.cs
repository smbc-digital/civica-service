using civica_service.Helpers.SessionProvider;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System.Threading.Tasks;

namespace civica_service.Controllers
{
    [Route("api/v1/[Controller]")]
    [Produces("application/json")]
    [ApiController]
    [TokenAuthentication]
    public class AvailabilityController : ControllerBase
    {
        private readonly ISessionProvider _sessionProvider;

        public AvailabilityController(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailability()
        {
            await _sessionProvider.GetSessionId();

            return Ok();
        }
    }
}