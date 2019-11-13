using System;
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

        [HttpGet]
        [Route("{personReference}/accounts")]
        public async Task<IActionResult> GetAccounts([FromRoute][Required]string personReference)
        {
            var response = await _civicaService.GetAccounts(personReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/transactions/{year}")]
        public async Task<IActionResult> GetAllTransactionsForYear([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required] int year)
        {
            _civicaService.GetCouncilTaxDetails(personReference, accountReference);
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
        [Route("{personReference}/properties/current")]
        public async Task<IActionResult> GetCurrentProperty([FromRoute][Required]string personReference)
        {
            var response = await _civicaService.GetCurrentProperty(personReference);

            return Ok(response);
        }

        [HttpGet]
        [Route("{personReference}/payments/{year}")]
        public async Task<IActionResult> GetPaymentSchedule([FromRoute][Required]string personReference, [FromRoute][Required]string year)
        {
            var response = await _civicaService.GetPaymentSchedule(personReference, year);

            return Ok(response);
        }
    }
}