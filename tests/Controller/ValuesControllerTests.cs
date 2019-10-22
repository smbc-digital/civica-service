using civica_service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockportGovUK.AspNetCore.Availability.Managers;
using Xunit;

namespace civica_service_tests.Controllers
{
    public class ValuesControllerTests
    {
        private readonly ValuesController _valuesController;
        private readonly Mock<IAvailabilityManager> _mockAvailabilityManager = new Mock<IAvailabilityManager>();

        public ValuesControllerTests()
        {
            _valuesController = new ValuesController(_mockAvailabilityManager.Object);
        }

        [Fact]
        public void Get_ShouldReturnOK()
        {
            // Act
            var response = _valuesController.Get();
            var statusResponse = response as OkObjectResult;
            
            // Assert
            Assert.NotNull(statusResponse);
            Assert.Equal(200, statusResponse.StatusCode);
        }

        [Fact]
        public async void Post_GivenFeatureToggleEnabled_ShouldReturnOK()
        {
            // Arrange
            _mockAvailabilityManager
                .Setup(_ => _.IsFeatureEnabled(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var response = await _valuesController.Post();
            var statusResponse = response as OkObjectResult;
            
            // Assert
            Assert.NotNull(statusResponse);
            Assert.Equal(200, statusResponse.StatusCode);
        }

        [Fact]
        public async void Post_GivenFeatureToggleDisabled_ShouldReturnNotFound()
        {
            // Arrange
            _mockAvailabilityManager
                .Setup(_ => _.IsFeatureEnabled(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var response = await _valuesController.Post();
            var statusResponse = response as NotFoundResult;
            
            // Assert
            Assert.NotNull(statusResponse);
            Assert.Equal(404, statusResponse.StatusCode);
        }
    }
}
