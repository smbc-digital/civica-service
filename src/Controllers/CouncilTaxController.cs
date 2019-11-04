using civica_service.Services;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace civica_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v2/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class CouncilTaxController : ControllerBase
    {
        private readonly ICivicaService _civicaService;

        public CouncilTaxController (ICivicaService civicaService)
        {
            _civicaService = civicaService;
        }

        [HttpGet]
        [Route("council-tax/{personReference}/details/transactions/{year}")]
        public async Task<IActionResult> GetAllTransactionsForYear([FromRoute][Required]string personReference, [FromRoute][Required] int year)
        {
            var response = await _civicaService.GetAllTransactionsForYear(personReference, year);

            return Ok(response);
        }
    }
}