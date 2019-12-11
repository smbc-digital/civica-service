using System.Collections.Generic;
using System.Net.Http;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Services;
using civica_service.Services.Models;
using civica_service.Utils.StorageProvider;
using civica_service.Utils.Xml;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StockportGovUK.AspNetCore.Gateways;
using StockportGovUK.NetStandard.Models.Civica.CouncilTax;
using StockportGovUK.NetStandard.Models.RevsAndBens;
using Xunit;
using CouncilTaxDocumentsResponse = StockportGovUK.NetStandard.Models.Civica.CouncilTax.CouncilTaxDocumentsResponse;

namespace civica_service_tests.Service
{
    public class CivicaServiceTests
    {
        private readonly CivicaService _civicaService;
        private readonly Mock<IGateway> _mockGateway = new Mock<IGateway>();
        private readonly Mock<IQueryBuilder> _mockQueryBuilder = new Mock<IQueryBuilder>();
        private readonly Mock<ISessionProvider> _mockSessionProvider = new Mock<ISessionProvider>();
        private readonly Mock<ICacheProvider> _mockCacheProvider = new Mock<ICacheProvider>();
        private readonly Mock<IXmlParser> _mockXmlParser = new Mock<IXmlParser>();
        private readonly Mock<ILogger<CivicaService>> _mockLogger = new Mock<ILogger<CivicaService>>();
        private const string SessionId = "test-session-id";

        public CivicaServiceTests()
        {
            _mockSessionProvider
                .Setup(_ => _.GetSessionId(It.IsAny<string>()))
                .ReturnsAsync(SessionId);

            _mockQueryBuilder
                .Setup(_ => _.Add(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_mockQueryBuilder.Object);

            _mockQueryBuilder
                .Setup(_ => _.Build())
                .Returns(string.Empty);

            _mockGateway
                .Setup(_ => _.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty)
                });

            _mockGateway
              .Setup(_ => _.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
              .ReturnsAsync(new HttpResponseMessage
              {
                  Content = new StringContent(string.Empty)
              });

            _civicaService = new CivicaService(_mockGateway.Object, _mockQueryBuilder.Object, _mockSessionProvider.Object, _mockCacheProvider.Object, _mockXmlParser.Object, _mockLogger.Object);
        }

        [Fact]
        public async void GetSessionId_ShouldCallSessionProvider()
        {
            // Act
            await _civicaService.GetSessionId("test");

            // Assert
            _mockSessionProvider.Verify(_ => _.GetSessionId("test"), Times.Once);
        }

        [Fact]
        public async void GetSessionId_ShouldReturnSessionId()
        {
            //Act
            var result = await _civicaService.GetSessionId("test-session-id");

            //Assert
            Assert.Equal(SessionId, result);
        }

        [Fact]
        public async void GetAccounts_ShouldCallCorrectGatewayUrl()
        {
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CtaxActDetails>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns( new CtaxActDetails
                    {
                        AccountStatus = "",
                        CtaxActAddress = "",
                        CtaxActRef = "",
                        CtaxBalance = ""
                    }
                );

            _ = await _civicaService.GetAccounts("");

            _mockXmlParser.Verify(
                _ => _.DeserializeDescendentsToIEnumerable<CtaxActDetails>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Add("docid", "ctxsel"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);

            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefits_ShouldCallGateway()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<BenefitsClaimsSummaryResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new BenefitsClaimsSummaryResponse
                {
                    Claims = new BenefitsClaimList
                    {
                        Summary = new List<BenefitsClaimSummary>()
                    }
                });

            // Act
            _ = await _civicaService.GetBenefits("");

            // Assert
            _mockXmlParser.Verify(
                _ => _.DeserializeXmlStringToType<BenefitsClaimsSummaryResponse>(It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "hbsel"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);
            _mockGateway.Verify(_ => _.PostAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async void GetBenefits_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<BenefitsClaimSummary>());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetBenefits(It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefits_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<BenefitsClaimsSummaryResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new BenefitsClaimsSummaryResponse
                {
                    Claims = new BenefitsClaimList
                    {
                        Summary = new List<BenefitsClaimSummary>()
                    }
                });

            // Act
            await _civicaService.GetBenefits((It.IsAny<string>()));

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public async void GetBenefitDetails_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new BenefitsClaim());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefitDetails_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<BenefitsClaim>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new BenefitsClaim());

            // Act
            await _civicaService.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetBenefitDetails_ShouldCallGateway()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<BenefitsClaim>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new BenefitsClaim());

            // Act
            _ = await _civicaService.GetBenefitDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockXmlParser.Verify(
                _ => _.DeserializeXmlStringToType<BenefitsClaim>(It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "hbdet"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("claimref", It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("placeref", It.IsAny<string>()), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetHousingBenefitPaymentHistory_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<PaymentDetail>());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetHousingBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);

        }

        [Fact]
        public async void GetHousingBenefitPaymentHistory_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<PaymentDetailsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new PaymentDetailsResponse
                {
                    PaymentList = new PaymentDetailList
                    {
                        PaymentDetails = new List<PaymentDetail>()
                    }
                });

            // Act
            await _civicaService.GetHousingBenefitPaymentHistory((It.IsAny<string>()));

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetHousingBenefitPaymentHistory_ShouldCallGateway()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<PaymentDetailsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new PaymentDetailsResponse
                {
                    PaymentList = new PaymentDetailList()
                    {
                        PaymentDetails = new List<PaymentDetail>()
                    }
                });

            // Act
            _ = await _civicaService.GetHousingBenefitPaymentHistory("");

            // Assert
            _mockXmlParser.Verify(
                _ => _.DeserializeXmlStringToType<PaymentDetailsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "hbpaydet"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("type", "prp"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxBenefitPaymentHistory_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<PaymentDetail>());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxBenefitPaymentHistory_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<PaymentDetailsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new PaymentDetailsResponse
                {
                    PaymentList = new PaymentDetailList
                    {
                        PaymentDetails = new List<PaymentDetail>()
                    }
                });

            // Act
            await _civicaService.GetCouncilTaxBenefitPaymentHistory((It.IsAny<string>()));

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxBenefitPaymentHistory_ShouldCallGateway()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<PaymentDetailsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new PaymentDetailsResponse
                {
                    PaymentList = new PaymentDetailList()
                    {
                        PaymentDetails = new List<PaymentDetail>()
                    }
                });

            _mockCacheProvider
                .Setup(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()));

            // Act
            _ = await _civicaService.GetCouncilTaxBenefitPaymentHistory(It.IsAny<string>());

            // Assert
            _mockXmlParser.Verify(
                _ => _.DeserializeXmlStringToType<PaymentDetailsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "hbpaydet"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("type", "ctp"), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetDocuments_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<CouncilTaxDocumentReference>());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetDocuments(It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetDocuments_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxDocumentsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new CouncilTaxDocumentsResponse
                {
                    DocumentList = new CouncilTaxDocumentReference[5]
                });

            // Act
            await _civicaService.GetDocuments((It.IsAny<string>()));

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetDocuments_ShouldCallGateway()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxDocumentsResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new CouncilTaxDocumentsResponse
                {
                    DocumentList = new CouncilTaxDocumentReference[5]
                });

            // Act

            _ = await _civicaService.GetDocuments(It.IsAny<string>());

            // Assert
            _mockXmlParser.Verify(
                _ => _.DeserializeDescendentsToIEnumerable<CouncilTaxDocumentReference>(It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "viewdoc"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("type", "all"), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);

            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetAccounts_ShouldGetModelFromCacheProvider()
        {
            var model = JsonConvert.SerializeObject(new List<CtaxActDetails>
            {
                new CtaxActDetails()
            });

            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            _ = await _civicaService.GetAccounts("");

            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
            _mockGateway.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetPropertiesOwned_ShouldCallCorrectGatewayUrl()
        {
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxPropertyDetails>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CouncilTaxPropertyDetails
                {
                    PropertyList = new civica_service.Services.Models.PropertyList()
                });

            _ = await _civicaService.GetPropertiesOwned("");

            _mockXmlParser.Verify(_ => _.DeserializeDescendentsToIEnumerable<Place>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Add("docid", "ctxprop"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("proplist", "y"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);

            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);

            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetPropertiesOwned_ShouldGetModelFromCacheProvider()
        {
            var model = JsonConvert.SerializeObject(new List<Place>());

            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            _ = await _civicaService.GetPropertiesOwned("");

            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
            _mockGateway.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetCurrentProperty_ShouldCallCorrectGatewayUrl()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeDescendentsToIEnumerable<Place>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Place>
                {
                    new Place(),
                    new Place
                    {
                        Status = "Current"
                    },
                    new Place()
                });

            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxAccountResponse>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CouncilTaxAccountResponse());

            // Act
            var response = await _civicaService.GetCurrentProperty("", "");

            // Assert
            _mockXmlParser.Verify(_ => _.DeserializeDescendentsToIEnumerable<Place>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "ctxprop"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("proplist", "y"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.AtLeastOnce);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.AtLeastOnce);
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.AtLeastOnce);
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));

            Assert.Equal("Current", response.Status);
        }

        [Fact]
        public async void GetCurrentProperty_ShouldGetModelFromCacheProvider()
        {
            var model = JsonConvert.SerializeObject(new Place());

            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            var response = await _civicaService.GetCurrentProperty("", "");

            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
            _mockGateway.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetCouncilTaxDetailsForYear_ShouldCallCorrectGatewayUrl()
        {
            var personReference = "test-person-ref";
            var accountReference = "test-account-ref";
            var year = "2019";

            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxAccountSummary>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CouncilTaxAccountSummary
                {
                    FinancialDetails = new FinancialDetails()
                });

            _ = await _civicaService.GetCouncilTaxDetailsForYear(personReference, accountReference, year);

            _mockXmlParser.Verify(_ => _.DeserializeXmlStringToType<CouncilTaxAccountSummary>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Add("docid", "ctxdet"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("actref", accountReference), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("year", year), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);

            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);

            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxDetailsForYear_ShouldGetModelFromCacheProvider()
        {
            var personReference = "test-person-ref";
            var accountReference = "test-account-ref";
            var year = "2019";

            var model = JsonConvert.SerializeObject(new RecievedYearTotal
            {
                TotalCharge = "400",
                TotalPayments = "500",
                BalanceOutstanding = "500",
                TotalBenefits = "500"
            });

            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);


            _ = await _civicaService.GetCouncilTaxDetailsForYear(personReference, accountReference, year);

            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
            _mockGateway.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetDocumentsWithAccountReference_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<CouncilTaxDocumentReference>());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetDocumentsWithAccountReference_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                 .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxDocumentsResponse>(It.IsAny<string>(),
                     It.IsAny<string>()))
                 .Returns(new CouncilTaxDocumentsResponse
                 {
                     DocumentList =
                     new[]{
                         new CouncilTaxDocumentReference(),
                         new CouncilTaxDocumentReference(),
                         new CouncilTaxDocumentReference(),
                         new CouncilTaxDocumentReference(),
                         new CouncilTaxDocumentReference()
                     }
                 });

            // Act
            await _civicaService.GetDocuments("test");
            await _civicaService.GetDocumentsWithAccountReference(It.IsAny<string>(), It.IsAny<string>());

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public async void GetCouncilTaxDetails_ShouldCallCacheProvider_WithGetStringAsync()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new CouncilTaxAccountResponse());
            _mockCacheProvider
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(model);

            // Act
            await _civicaService.GetCouncilTaxDetails(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            _mockCacheProvider.Verify(_ => _.GetStringAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetCouncilTaxDetails_ShouldCallCacheProvider_WithSetStringAsync()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CouncilTaxAccountResponse>(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new CouncilTaxAccountResponse());

            // Act
            await _civicaService.GetCouncilTaxDetails(It.IsAny<string>(), It.IsAny<string>());

            //Assert
            _mockCacheProvider.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
