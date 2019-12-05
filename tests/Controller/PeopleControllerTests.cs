using civica_service.Controllers;
using civica_service.Services;
using Moq;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;
using System.Collections.Generic;
using Xunit;

namespace civica_service_tests.Controller
{
    public class PeopleControllerTests
    {
        private readonly PeopleController _controller;
        private readonly Mock<ICivicaService> _mockService = new Mock<ICivicaService>();

        public PeopleControllerTests()
        {
            _controller = new PeopleController(_mockService.Object);
        }

        [Fact]
        public async void GetSessionId_ShouldReturn200()
        {
            // Arrange
            _mockService
                .Setup(_ => _.GetSessionId(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());

            // Act
            await _controller.GetSessionId(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetSessionId(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void IsBenefitsClaimant_ShouldReturn200()
        {
            // Arrange
            _mockService
                .Setup(_ => _.IsBenefitsClaimant(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<bool>());

            // Act
            await _controller.IsBenefitsClaimant(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.IsBenefitsClaimant(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefits_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetBenefits(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<BenefitsClaimSummary>>());

            // Act
            await _controller.GetBenefits(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetBenefits(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefitDetails_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<BenefitsClaim>());

            // Act
            await _controller.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetHousingBenefitPaymentHistory_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetHousingBenefitPaymentHistory(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<PaymentDetail>>());

            // Act
            await _controller.GetHousingBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetHousingBenefitPaymentHistory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxBenefitPaymentHistory_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<PaymentDetail>>());

            // Act
            await _controller.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetAccounts_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetAccounts(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<CtaxActDetails>>());

            // Act
            await _controller.GetAccounts(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetAccounts(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetAccount_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCouncilTaxDetails(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<CouncilTaxAccountResponse>());

            // Act
            await _controller.GetAccount(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCouncilTaxDetails(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetAccountDetailsForYear_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCouncilTaxDetailsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<RecievedYearTotal>());

            // Act
            await _controller.GetAccountDetailsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCouncilTaxDetailsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetAllTransactionsForYear_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetAllTransactionsForYear(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<List<Transaction>>());

            // Act
            await _controller.GetAllTransactionsForYear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            // Assert
            _mockService.Verify(_ => _.GetAllTransactionsForYear(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async void GetDocumentsWithAccountReference_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<CouncilTaxDocumentReference>>());

            // Act
            await _controller.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetDocuments_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetDocuments(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<CouncilTaxDocumentReference>>());

            // Act
            await _controller.GetDocuments(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetDocuments(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetPropertiesOwned_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetPropertiesOwned(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<List<Place>>());

            // Act
            await _controller.GetPropertiesOwned(It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetPropertiesOwned(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCurrentProperty_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetCurrentProperty(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Place>());

            // Act
            await _controller.GetCurrentProperty(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockService.Verify(_ => _.GetCurrentProperty(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetPaymentSchedule_ShouldReturn200()
        {
            // Arrange
            _mockService.Setup(_ => _.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<CouncilTaxPaymentScheduleResponse>());

            // Act
            await _controller.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>());

            // Assert
            _mockService.Verify(_ => _.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
