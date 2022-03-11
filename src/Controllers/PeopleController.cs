using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using civica_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class PeopleController : ControllerBase
    {
        private readonly ICivicaService _civicaService;
        private readonly ILogger<PeopleController> _logger;

        public PeopleController(ICivicaService civicaService, ILogger<PeopleController> logger)
        {
            _civicaService = civicaService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{personReference}/session-id")]
        public async Task<IActionResult> GetSessionId([FromRoute] [Required] string personReference)
        {
            string sessionId = await _civicaService.GetSessionId(personReference);

            return Ok(sessionId);
        }

        [HttpGet]
        [Route("{personReference}/is-benefits-claimant")]
        public async Task<IActionResult> IsBenefitsClaimant([FromRoute][Required]string personReference)
        {
            bool isBenefitClaimant = await _civicaService.IsBenefitsClaimant(personReference);

            return Ok(isBenefitClaimant);
        }

        [HttpGet]
        [Route("{personReference}/benefits")]
        public async Task<IActionResult> GetBenefits([FromRoute][Required] string personReference)
        {
            List<BenefitsClaimSummary> benefits = await _civicaService.GetBenefits(personReference);

            return Ok(benefits);
        }

        [HttpGet]
        [Route("{personReference}/benefits/{claimReference}/{placeReference}")]
        public async Task<IActionResult> GetBenefitDetails([FromRoute][Required] string personReference, [FromRoute][Required] string claimReference, [FromRoute][Required] string placeReference)
        {
            BenefitsClaim benefitDetails = await _civicaService.GetBenefitDetails(personReference, claimReference, placeReference);

            return Ok(benefitDetails);
        }

        [HttpGet]
        [Route("{personReference}/benefits/housing")]
        public async Task<IActionResult> GetHousingBenefitPaymentHistory([FromRoute][Required] string personReference)
        {
            List<PaymentDetail> paymentHistory = await _civicaService.GetHousingBenefitPaymentHistory(personReference);

            return Ok(paymentHistory);
        }

        [HttpGet]
        [Route("{personReference}/benefits/council-tax")]
        public async Task<IActionResult> GetCouncilTaxBenefitPaymentHistory([FromRoute][Required] string personReference)
        {
            List<PaymentDetail> paymentHistory = await _civicaService.GetCouncilTaxBenefitPaymentHistory(personReference);

            return Ok(paymentHistory);
        }

        [HttpGet]
        [Route("{personReference}/accounts")]
        public async Task<IActionResult> GetAccounts([FromRoute][Required]string personReference)
        {
            List<CtaxActDetails> accounts = await _civicaService.GetAccounts(personReference);

            return Ok(accounts);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}")]
        public async Task<IActionResult> GetAccount([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference)
        {
            CouncilTaxAccountResponse councilTaxDetails = await _civicaService.GetCouncilTaxDetails(personReference, accountReference);

            return Ok(councilTaxDetails);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/{year}")]
        public async Task<IActionResult> GetAccountDetailsForYear([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required] string year)
        {
            ReceivedYearTotal councilTaxDetailsForYear = await _civicaService.GetCouncilTaxDetailsForYear(personReference, accountReference, year);

            return Ok(councilTaxDetailsForYear);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/transactions/{year}")]
        public async Task<IActionResult> GetAllTransactionsForYear([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required] int year)
        {
            List<Transaction> transactionsForYear = await _civicaService.GetAllTransactionsForYear(personReference, accountReference, year);

            return Ok(transactionsForYear);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/documents")]
        public async Task<IActionResult> GetDocumentsWithAccountReference([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference)
        {
            List<CouncilTaxDocumentReference> documents = await _civicaService.GetDocumentsWithAccountReference(personReference, accountReference);

            return Ok(documents);
        }

        [HttpGet]
        [Route("{personReference}/documents")]
        public async Task<IActionResult> GetDocuments([FromRoute][Required]string personReference)
        {
            List<CouncilTaxDocumentReference> documents = await _civicaService.GetDocuments(personReference);

            return Ok(documents);
        }

        [HttpGet]
        [Route("{personReference}/properties")]
        public async Task<IActionResult> GetPropertiesOwned([FromRoute][Required]string personReference)
        {
            List<Place> properties = await _civicaService.GetPropertiesOwned(personReference);

            return Ok(properties);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/properties/current")]
        public async Task<IActionResult> GetCurrentProperty([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference)
        {
            Place property = await _civicaService.GetCurrentProperty(personReference, accountReference);

            return Ok(property);
        }

        [HttpGet]
        [Route("{personReference}/payments/{year}")]
        public async Task<IActionResult> GetPaymentSchedule([FromRoute][Required]string personReference, [FromRoute][Required]int year)
        {
            List<Installment> paymentSchedule = await _civicaService.GetPaymentSchedule(personReference, year);

            return Ok(paymentSchedule);
        }

        [HttpGet]
        [Route("{personReference}/accounts/{accountReference}/documents/{documentId}")]
        public async Task<IActionResult> GetDocumentForAccount([FromRoute][Required]string personReference, [FromRoute][Required]string accountReference, [FromRoute][Required]string documentId)
        {
            byte[] document = await _civicaService.GetDocumentForAccount(personReference, accountReference, documentId);

            if (document == null)
            {
                _logger.LogWarning($"Document {documentId} is null for Person {personReference} with Account {accountReference}");
                return NotFound();
            }

            if (document.Length.Equals(0))
            {
                _logger.LogWarning($"Document {documentId} length is 0 for Person {personReference} with Account {accountReference}");
                return NoContent();
            }
            
            return File(document, "application/pdf", "download.pdf");
        }
    }
}