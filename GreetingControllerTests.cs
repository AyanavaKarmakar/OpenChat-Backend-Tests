using Microsoft.AspNetCore.Mvc;
using OpenChat.Greeting.Controllers;

namespace OpenChat_Backend.Greeting.Controllers.Tests
{
    public class GreetingControllerTests
    {
        [Fact]
        public void Get_IsResultOk()
        {
            // Arrange
            var controller = new GreetingController();

            // Act
            var response = controller.Get();

            // Assert
            Assert.NotNull(response);

            var okResponse = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public void Get_IsResultOk_WithMessage()
        {
            // Arrange
            var controller = new GreetingController();

            // Act
            var response = controller.Get();

            // Assert
            Assert.NotNull(response);

            var okResponse = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, okResponse.StatusCode);

            // is this really how it's supposed to be done?
            var message = Assert.IsType<string>(
                okResponse.Value?.GetType().GetProperty("message")?.GetValue(okResponse.Value));

            Assert.Equal("Welcome to OPENCHAT!", message);
        }
    }
}