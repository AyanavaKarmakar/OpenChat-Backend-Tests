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
    }
}