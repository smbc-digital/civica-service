using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.StorageProvider;
using civica_service.Utils.Xml;
using Microsoft.Extensions.Options;
using Moq;
using StockportGovUK.AspNetCore.Gateways;
using System;
using System.Net.Http;
using Xunit;

namespace civica_service_tests.Helpers
{
    public class SessionProviderTests
    {
        private readonly SessionProvider _sessionProvider;
        private readonly Mock<IGateway> _mockGateway = new Mock<IGateway>();
        private readonly Mock<IQueryBuilder> _mockQueryBuilder = new Mock<IQueryBuilder>();
        private readonly Mock<IOptions<SessionConfiguration>> _configuration = new Mock<IOptions<SessionConfiguration>>();
        private readonly Mock<ICacheProvider> _distributedCache = new Mock<ICacheProvider>();
        private readonly Mock<IXmlParser> _mockXmlParser = new Mock<IXmlParser>();

        public SessionProviderTests()
        {
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

            _configuration.Setup(_ => _.Value).Returns(new SessionConfiguration
            {
                Username = "username-test",
                Password = "password-test"
            });

            _sessionProvider = new SessionProvider(_mockGateway.Object, _mockQueryBuilder.Object, _configuration.Object, _distributedCache.Object, _mockXmlParser.Object);
        }

        [Fact]
        public async void GetSessionId_ShouldCallDistributedCache_WithGetStringAsync()
        {
            //Arrange
            _distributedCache
                .Setup(_ => _.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync("test");

            // Act
            await _sessionProvider.GetSessionId("test");

            // Assert
            _distributedCache.Verify(_ => _.GetStringAsync(It.IsAny<string>()),Times.Once);
        }

        [Fact]
        public async void GetSessionId_ShouldCallDistributedCache_WithSetStringAsync()
        {
            //Arrange
            _distributedCache
                .Setup(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()));

            _mockXmlParser
               .Setup(_ => _.DeserializeXmlStringToType<SessionIdModel>(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new SessionIdModel
               {
                   Result = new Result
                   {
                       SessionID = "session-test",
                       ErrorCode = new ErrorCode
                       {
                           Text = "5"
                       }
                   }
               });

            _mockXmlParser
               .Setup(_ => _.DeserializeXmlStringToType<SetPersonModel>(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new SetPersonModel
               {
                   ErrorCode = new ErrorCode
                   {
                       Description = "Person Reference Set"
                   }
               });

            // Act
            await _sessionProvider.GetSessionId(It.IsAny<string>());

            // Assert
            _distributedCache.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetSessionId_ShouldCallTheGateway()
        {
            // Arrange
            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<SessionIdModel>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new SessionIdModel
                {
                    Result = new Result
                    {
                        SessionID = "session-test",
                        ErrorCode = new ErrorCode
                        {
                            Text = "5"
                        }
                    }
                });

            _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<SetPersonModel>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new SetPersonModel
                {
                    ErrorCode = new ErrorCode
                    {
                        Description = "Person Reference Set"
                    }
                });

            // Act
            await _sessionProvider.GetSessionId("");

            // Assert
            _mockXmlParser.Verify(
            _ => _.DeserializeXmlStringToType<SessionIdModel>(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("docid", "crmlogin"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("userid", "username-test"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Add("password", "password-test"), Times.Once);
            _mockQueryBuilder.Verify(_ => _.Build(), Times.Exactly(2));
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async void GetSessionId_ShouldThrowException()
        {
            // Arrange
            _mockXmlParser
               .Setup(_ => _.DeserializeXmlStringToType<SessionIdModel>(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new SessionIdModel
               {
                   Result = new Result
                   {
                       SessionID = "session-test",
                       ErrorCode = new ErrorCode
                       {
                           Text = "6"
                       }
                   }
               });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sessionProvider.GetSessionId("session-test"));
        }

        [Fact]
        public async void GetSessionId_ShouldThrowException_WhenSessionIdIsNullOrEmpty()
        {
            // Arrange
            _mockXmlParser
               .Setup(_ => _.DeserializeXmlStringToType<SessionIdModel>(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new SessionIdModel
               {
                   Result = new Result
                   {
                       SessionID = string.Empty,
                       ErrorCode = new ErrorCode
                       {
                           Text = "6"
                       }
                   }
               });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sessionProvider.GetSessionId(string.Empty));
        }

        [Fact]
        public async void GetSessionId_ShouldThrowException_WhenErrorCodeDescriptionDoesNotMatch()
        {
            // Arrange
            _mockXmlParser
               .Setup(_ => _.DeserializeXmlStringToType<SessionIdModel>(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new SessionIdModel
               {
                   Result = new Result
                   {
                       SessionID = string.Empty,
                       ErrorCode = new ErrorCode
                       {
                           Text = "6"
                       }
                   }
               });

             _mockXmlParser
                .Setup(_ => _.DeserializeXmlStringToType<SetPersonModel>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new SetPersonModel
                {
                    ErrorCode = new ErrorCode
                    {
                        Description = "TEST"
                    }
                });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sessionProvider.GetSessionId(string.Empty));
        }
    }
}
