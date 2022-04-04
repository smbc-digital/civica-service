using civica_service.Controllers;
using civica_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;
using System.Collections.Generic;
using Xunit;
using PersonName = StockportGovUK.NetStandard.Models.Civica.CouncilTax.PersonName;

namespace civica_service_tests.Controller
{
    public class PeopleControllerTests
    {
        private readonly PeopleController _controller;
        private readonly Mock<ICivicaService> _mockService = new Mock<ICivicaService>();
        private readonly Mock<ILogger<PeopleController>> _mockLogger = new Mock<ILogger<PeopleController>>();

        public PeopleControllerTests()
        {
            _mockService
                .Setup(_ => _.GetSessionId(It.IsAny<string>()))
                .ReturnsAsync("sessionId");

            _mockService
                .Setup(_ => _.IsBenefitsClaimant(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockService
                .Setup(_ => _.GetBenefits(It.IsAny<string>()))
                .ReturnsAsync(new List<BenefitsClaimSummary>());

            _mockService
                .Setup(_ => _.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new BenefitsClaim());

            _mockService
                .Setup(_ => _.GetHousingBenefitPaymentHistory(It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentDetail>());

            _mockService
                .Setup(_ => _.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentDetail>());

            _mockService
                .Setup(_ => _.GetPerson(It.IsAny<string>()))
                .ReturnsAsync(new PersonName());

            _controller = new PeopleController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async void GetSessionId_ShouldCallService()
        {
            // Act
            await _controller.GetSessionId(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetSessionId(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetSessionId_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetSessionId(It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void IsBenefitsClaimant_ShouldCallService()
        {
            // Act
            await _controller.IsBenefitsClaimant(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.IsBenefitsClaimant(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void IsBenefitsClaimant_ShouldReturnOk()
        {
            // Act
            var result = await _controller.IsBenefitsClaimant(It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetBenefits_ShouldCallService()
        {
            // Act
            await _controller.GetBenefits(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetBenefits(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefits_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetBenefits(It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetBenefitDetails_ShouldCallService()
        {
            // Act
            await _controller.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefitDetails_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetHousingBenefitPaymentHistory_ShouldCallService()
        {
            // Act
            await _controller.GetHousingBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetHousingBenefitPaymentHistory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetHousingBenefitPaymentHistory_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetHousingBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetCouncilTaxBenefitPaymentHistory_ShouldCallService()
        {
            // Act
            await _controller.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxBenefitPaymentHistory_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetAccounts_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetAccounts(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<CtaxActDetails>>());

            // Act
            var result = await _controller.GetAccounts(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetAccounts(It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetPerson_ShouldCallService()
        {
            // Act
            await _controller.GetPerson(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetPerson(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetPerson_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetPerson(It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetAccount_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCouncilTaxDetails(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<CouncilTaxAccountResponse>());

            // Act
            var result = await _controller.GetAccount(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCouncilTaxDetails(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetAccountDetailsForYear_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCouncilTaxDetailsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<ReceivedYearTotal>());

            // Act
            var result = await _controller.GetAccountDetailsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCouncilTaxDetailsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetAllTransactionsForYear_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetAllTransactionsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<List<Transaction>>());

            // Act
            var result = await _controller.GetAllTransactionsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            // Assert
            _mockService.Verify(_ => _.GetAllTransactionsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetDocumentsWithAccountReference_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<CouncilTaxDocumentReference>>());

            // Act
            var result = await _controller.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetDocuments_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocuments(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<CouncilTaxDocumentReference>>());

            // Act
            var result = await _controller.GetDocuments(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetDocuments(It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetPropertiesOwned_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetPropertiesOwned(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<Place>>());

            // Act
            var result = await _controller.GetPropertiesOwned(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetPropertiesOwned(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetCurrentProperty_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCurrentProperty(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Place>());

            // Act
            var result = await _controller.GetCurrentProperty(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCurrentProperty(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetPaymentSchedule_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<List<Installment>>());

            // Act
            var result = await _controller.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>());

            // Assert
            _mockService.Verify(_ => _.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async void GetDocumentForAccount_ShouldCallService()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new byte[5]);

            // Act
            var result = await _controller.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            var actionResult = Assert.IsType<FileContentResult>(result);
            Assert.NotNull(actionResult);
        }

        [Fact]
        public async void GetDocumentForAccount_ShouldReturnNotFound_IfResponseNull()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((byte[])null);

            // Act
            var result = await _controller.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, actionResult.StatusCode);
        }

        [Fact]
        public async void GetDocumentForAccount_ShouldReturnNoContent_IfResponseEmpty()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new byte[0]);
            
            // Act
            var result = await _controller.GetDocumentForAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, actionResult.StatusCode);
        }
    }
}
