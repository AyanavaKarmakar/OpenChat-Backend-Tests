using Chat.Models;
using Chat.UserDto;
using Microsoft.AspNetCore.Mvc;
using OpenChat.Auth.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace OpenChat.Auth.Tests
{
    public class AuthControllerTests
    {
        private readonly ChatDB _context;
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDB>()
                .UseInMemoryDatabase(databaseName: "ChatDB")
                .Options;

            _context = new ChatDB(options);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("AppSettings:Token", "mysupersecretkey")
                })
                .Build();

            _configuration = configuration;
        }

        private async Task<ChatDB> ArrangeTestData()
        {
            // Clear the database to ensure that there are no duplicate keys
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            var users = new List<UserEntity>
            {
                new UserEntity
                {
                    Id = 1,
                    Username = "johndoe",
                    PasswordHash = new byte[64],
                    PasswordSalt = new byte[128]
                },
                new UserEntity
                {
                    Id = 2,
                    Username = "janedoe",
                    PasswordHash = new byte[64],
                    PasswordSalt = new byte[128]
                }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            return _context;
        }

        [Fact]
        public async Task Register_ReturnsBadRequestObjectResult_WhenUsernameIsAlreadyTaken()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new AuthController(context, _configuration);
                var request = new UserDto
                {
                    Username = "johndoe",
                    Password = "testpassword"
                };

                // Act
                var result = await controller.Register(request);

                // Assert
                Assert.NotNull(result);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(400, badRequestResult.StatusCode);

                var message = Assert.IsType<string>(
                    badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value));
                Assert.Equal("Username already taken", message);
            }
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var controller = new AuthController(_context, _configuration);

            var request = new UserDto
            {
                Username = "janedoe",
                Password = "testpassword"
            };

            // Act
            var result = await controller.Register(request);

            // Assert
            Assert.NotNull(result);
        }
    }
}
