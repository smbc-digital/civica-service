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
        private readonly CivicaService _service;
        private readonly Mock<IGateway> _mockGateway = new Mock<IGateway>();
        private readonly Mock<IQueryBuilder> _mockQueryBuilder = new Mock<IQueryBuilder>();
        private readonly Mock<ISessionProvider> _mockSessionProvider = new Mock<ISessionProvider>();
        private readonly Mock<IDistributedCache> _mockCacheProvider = new Mock<IDistributedCache>();

        public CivicaServiceTests()
        {
            _service = new CivicaService(_mockGateway.Object, _mockQueryBuilder.Object, _mockSessionProvider.Object, _mockCacheProvider.Object);
        }

        [Fact]
        public async void GetSessionId_ShouldCallSessionProvider()
        {
            //Arrange
            _mockSessionProvider
                .Setup(_ => _.GetSessionId(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());

            //Act
            await _service.GetSessionId(It.IsAny<string>());

            //Assert
            _service.Verify(_ => _.GetSessionId(It.IsAny<string>()), Times.Once);
            _mockService.Verify(_ => _.GetPaymentSchedule(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
