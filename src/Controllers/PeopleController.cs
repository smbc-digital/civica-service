using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using civica_service.Services;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;

namespace civica_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class PeopleController : ControllerBase
    {
        private readonly ICivicaService _civicaService;

        public PeopleController(ICivicaService civicaService)
        {
            _civicaService = civicaService;
        }

        [HttpGet]
        [Route("{personReference}/is-benefits-claimant")]
        public async Task<IActionResult> IsBenefitsClaimant([FromRoute][Required]string personReference)
        {
            var model = await _civicaService.IsBenefitsClaimant(personReference);

            return StatusCode(StatusCodes.Status200OK, model);
        }

        [HttpGet]
        [Route("{personReference}/benefits")]
        public async Task<IActionResult> GetBenefits([FromRoute][Required]string personReference)
        {
            var model = await _civicaService.GetBenefits(personReference);

            return StatusCode(StatusCodes.Status200OK, model);
        }
    }
}