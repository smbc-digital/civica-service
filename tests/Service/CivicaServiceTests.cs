using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using StockportGovUK.AspNetCore.Gateways;
using Xunit;

namespace civica_service_tests.Service
{
    public class CivicaServiceTests
    {
        private readonly CivicaService _civicaService;
        private readonly Mock<IGateway> _mockGateway = new Mock<IGateway>();
        private readonly Mock<IQueryBuilder> _mockQueryBuilder = new Mock<IQueryBuilder>();
        private readonly Mock<ISessionProvider> _mockSessionProvider = new Mock<ISessionProvider>();
        private readonly Mock<IDistributedCache> _mockCacheProvider = new Mock<IDistributedCache>();

        public CivicaServiceTests()
        {
            _civicaService = new CivicaService(_mockGateway.Object, _mockQueryBuilder.Object, _mockSessionProvider.Object, _mockCacheProvider.Object);
        }

        [Fact]
        public async void GetSessionId_ShouldCallSessionProvider()
        {
            //Arrange
            _mockSessionProvider
                .Setup(_ => _.GetSessionId(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());

            //Act
            await _civicaService.GetSessionId(It.IsAny<string>());

            //Assert
            _mockSessionProvider.Verify(_ => _.GetSessionId(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetSessionId_ShouldReturnPersonReference()
        {
            //Arrange
            _mockSessionProvider
                .Setup(_ => _.GetSessionId(It.IsAny<string>()))
                    .ReturnsAsync(It.IsAny<string>());

            //Act
            var result = await _civicaService.GetSessionId("test");

            //Assert
            Assert.Equal(It.IsAny<string>(), result);
        }

        [Fact]
        public async void IsBenefitsClaimant_ShouldReturnIsBenefitsClaimant()
        {
            //Arrange

          
        }
    }
}
