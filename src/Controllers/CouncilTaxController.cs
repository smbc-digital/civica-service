using civica_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System.ComponentModel.DataAnnotations;

namespace civica_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v2/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class CouncilTaxController : Controller
    {

        private readonly ICouncilTaxService _councilTaxService;
        private readonly ILogger<CouncilTaxController> _logger;

        public CouncilTaxController (ICouncilTaxService councilTaxService, ILogger<CouncilTaxController> logger)
        {
            _councilTaxService = councilTaxService;
            _logger = logger;
        }

        [HttpGet]
        [Route("council-tax/{personReference}/details/{accountReference}/previous-payments/{year}")]

        public ActionResult GetCouncilTaxPreviousPayments([FromRoute][Required] string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required] int year)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Received an invalid council tax request with the following error messages: {0}");

                return BadRequest(ModelState);
            }

            var response = _councilTaxService.GetCouncilTaxTransactions(personReference, accountReference, year);

            if(response == null)
            {
                _logger.LogWarning("Could not match the Council request with ref: {0}");

                return NotFound();
            }

            return Ok(response);
        }
    }
}