using System.Collections.Generic;
using System.Fabric;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Xml;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Services;
using civica_service.Services.Models;
using civica_service.Utils.Xml;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using StockportGovUK.AspNetCore.Gateways;
using StockportGovUK.NetStandard.Models.Models.Civica.CouncilTax;
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
        private readonly Mock<IXmlParser> _mockXmlParser = new Mock<IXmlParser>();
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

            _civicaService = new CivicaService(_mockGateway.Object, _mockQueryBuilder.Object, _mockSessionProvider.Object, _mockCacheProvider.Object, _mockXmlParser.Object);
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

        [Fact]
        public async void GetAccounts_ShouldCallCorrectGatewayUrl()
        {
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<CtaxSelectDoc>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CtaxSelectDoc
                {
                    CtaxActList = new CtaxActList()
                });

            _mockGateway
                .Setup(_ => _.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty)
                });

            _ = await _civicaService.GetAccounts("");

            _mockXmlParser.Verify(_ => _.DeserializeXmlStringToType<CtaxSelectDoc>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            _mockQueryBuilder.Verify(_ => _.Add("docid", "ctxsel"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("sessionId", SessionId), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Once);

            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
