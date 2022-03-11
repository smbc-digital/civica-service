using System;
using System.Threading.Tasks;
using civica_service.Controllers;
using civica_service.Helpers.SessionProvider;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace civica_service_tests.Controller
{
    public class AvailabilityControllerTests
    {
        private readonly Mock<ISessionProvider> _mockSessionProvider = new Mock<ISessionProvider>();
        private readonly AvailabilityController _controller;

        public AvailabilityControllerTests()
        {
            _mockSessionProvider
                .Setup(_ => _.GetSessionId())
                .ReturnsAsync("123");

            _mockSessionProvider
                .Setup(_ => _.GetAnonymousSessionId())
                .ReturnsAsync("456");

            _controller = new AvailabilityController(_mockSessionProvider.Object);
        }

        [Fact]
        public async Task GetAvailability_ShouldCallSessionProvider()
        {
            // Act
            await _controller.GetAvailability();

            // Assert
            _mockSessionProvider.Verify(_ => _.GetSessionId(), Times.Once);
        }

        [Fact]
        public async Task GetAvailability_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetAvailability();

            // Assert
            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAvailability_ShouldCatchException_Return424()
        {
            // Arrange
            _mockSessionProvider
                .Setup(_ => _.GetSessionId())
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAvailability();

            // Assert
            var actionResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(424, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAnonymousAvailability_ShouldCallSessionProvider()
        {
            // Act
            await _controller.GetAnonymousAvailability();

            // Assert
            _mockSessionProvider.Verify(_ => _.GetAnonymousSessionId(), Times.Once);
        }

        [Fact]
        public async Task GetAnonymousAvailability_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetAnonymousAvailability() as OkResult;

            // Assert
            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAnonymousAvailability_ShouldCatchException_Return424()
        {
            // Arrange
            _mockSessionProvider
                .Setup(_ => _.GetAnonymousSessionId())
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAnonymousAvailability();

            // Assert
            var actionResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(424, actionResult.StatusCode);
        }
    }
}
