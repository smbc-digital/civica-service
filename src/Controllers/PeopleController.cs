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
        [Route("{personReference}/session-id")]
        public async Task<IActionResult> GetSessionId([FromRoute] [Required] string personReference)
        {
            var sessionId = await _civicaService.GetSessionId(personReference);

            return StatusCode(StatusCodes.Status200OK, sessionId);
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
        public async Task<IActionResult> GetBenefits([FromRoute][Required] string personReference)
        {
            var model = await _civicaService.GetBenefits(personReference);

            return StatusCode(StatusCodes.Status200OK, model);
        }

        [HttpGet]
        [Route("{personReference}/benefits/{claimReference}/{placeReference}")]
        public async Task<IActionResult> GetBenefitDetails([FromRoute][Required] string personReference, [FromRoute][Required] string claimReference, [FromRoute][Required] string placeReference)
        {
            var model = await _civicaService.GetBenefitDetails(personReference, claimReference, placeReference);

            return StatusCode(StatusCodes.Status200OK, model);
        }

        [HttpGet]
        [Route("{personReference}/benefits/housing")]
        public async Task<IActionResult> GetHousingBenefitPaymentHistory([FromRoute][Required] string personReference)
        {
            var model = await _civicaService.GetHousingBenefitPaymentHistory(personReference);

            return StatusCode(StatusCodes.Status200OK, model);
        }

        [HttpGet]
        [Route("{personReference}/benefits/council-tax")]
        public async Task<IActionResult> GetCouncilTaxBenefitPaymentHistory([FromRoute][Required] string personReference)
        {
            var model = await _civicaService.GetCouncilTaxBenefitPaymentHistory(personReference);

            return StatusCode(StatusCodes.Status200OK, model);
        }

        [HttpGet]
        [Route("{personReference}/accounts")]
        public async Task<IActionResult> GetAccounts([FromRoute][Required]string personReference)
        {
            var response = await _civicaService.GetAccounts(personReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}")]
        public async Task<IActionResult> GetAccount([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference)
        {
            var response = await _civicaService.GetCouncilTaxDetails(personReference, accountReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/{year}")]
        public async Task<IActionResult> GetAccountDetailsForYear([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required] string year)
        {
            var response = await _civicaService.GetCouncilTaxDetailsForYear(personReference, accountReference, year);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/transactions/{year}")]
        public async Task<IActionResult> GetAllTransactionsForYear([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required] int year)
        {
            var response = await _civicaService.GetAllTransactionsForYear(personReference, year);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/documents")]
        public async Task<IActionResult> GetDocumentsWithAccountReference([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference)
        {
            var response = await _civicaService.GetDocumentsWithAccountReference(personReference, accountReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/documents")]
        public async Task<IActionResult> GetDocuments([FromRoute][Required]string personReference)
        {
            var response = await _civicaService.GetDocuments(personReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/properties")]
        public async Task<IActionResult> GetPropertiesOwned([FromRoute][Required]string personReference)
        {
            var response = await _civicaService.GetPropertiesOwned(personReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/properties/current")]
        public async Task<IActionResult> GetCurrentProperty([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference)
        {
            var response = await _civicaService.GetCurrentProperty(personReference, accountReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/payments/{year}")]
        public async Task<IActionResult> GetPaymentSchedule([FromRoute][Required]string personReference, [FromRoute][Required]int year)
        {
            var response = await _civicaService.GetPaymentSchedule(personReference, year);

            return Ok(response);
        }
    }
}