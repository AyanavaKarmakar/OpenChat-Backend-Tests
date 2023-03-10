using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using OpenChat.Message.Controllers;
using Microsoft.EntityFrameworkCore;

namespace OpenChat_Backend.Messages.Controllers.Tests
{
    public class MessagesControllerTests
    {
        private readonly DbContextOptions<ChatDB> options = new DbContextOptionsBuilder<ChatDB>()
                            .UseInMemoryDatabase(databaseName: "TestDB")
                            .Options;

        private async Task<ChatDB> ArrangeTestData()
        {
            var context = new ChatDB(options);

            // Clear the database to ensure that there are no duplicate keys
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var messages = new List<Message>
            {
                new Message
                {
                    Id = 1,
                    Sender = "user1",
                    MessageContent = "Hello",
                    Timestamp = DateTime.Now
                },
                new Message
                {
                    Id = 2,
                    Sender = "user2",
                    MessageContent = "Hi",
                    Timestamp = DateTime.Now.AddSeconds(5)
                },
                new Message
                {
                    Id = 3,
                    Sender = "user1",
                    MessageContent = "How are you?",
                    Timestamp = DateTime.Now.AddSeconds(10)
                }
            };

            context.Messages.AddRange(messages);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetAllMessages_ReturnsOkObjectResult_WithListOfMessages()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);

                // Act
                var result = await controller.GetAllMessages();

                // Assert
                Assert.NotNull(result);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                var messages = Assert.IsAssignableFrom<IEnumerable<Message>>(okResult.Value);
                Assert.Equal(3, messages.Count());
                Assert.Equal("How are you?", messages.First().MessageContent);
            }
        }

        [Fact]
        public async Task GetMessageById_WithValidId_ReturnsOkObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var messageId = 1;

                // Act
                var result = await controller.GetMessageById(messageId);

                // Assert
                Assert.NotNull(result);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                var message = Assert.IsType<Message>(okResult.Value);
                Assert.Equal("Hello", message.MessageContent);
            }
        }

        [Fact]
        public async Task GetMessagebyId_WithInvalidId_ReturnsNotFoundObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var messageId = 100;

                // Act
                var result = await controller.GetMessageById(messageId);

                // Assert
                Assert.NotNull(result);

                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);

                var message = Assert.IsType<string>(
                    notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
                Assert.Equal("Message not found", message);
            }
        }

        [Fact]
        public async Task CreateMessage_WithValidMessage_ReturnsOkObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var message = new Message
                {
                    Id = 4,
                    Sender = "user3",
                    MessageContent = "Hello, World!",
                    Timestamp = DateTime.Now
                };

                // Act
                var result = await controller.CreateMessage(message);

                // Aseert
                Assert.NotNull(result);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                var createdMessage = Assert.IsType<Message>(okResult.Value);
                Assert.Equal("Hello, World!", createdMessage.MessageContent);
            }
        }

        [Fact]
        public async Task CreateMessage_WithNullMessageObject_ReturnsBadRequest_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var blankMessage = null as Message;

                // Act
                var result = await controller.CreateMessage(blankMessage);

                // Assert
                Assert.NotNull(result);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(400, badRequestResult.StatusCode);

                var message = Assert.IsType<string>(
                    badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value));
                Assert.Equal("Message object is null", message);
            }
        }

        [Fact]
        public async Task DeleteMessage_WithValidId_ReturnsOkObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var messageId = 1;

                // Act
                var result = await controller.DeleteMessage(messageId);

                // Assert
                Assert.NotNull(result);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                var message = Assert.IsType<string>(
                    okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value));
                Assert.Equal("Message deleted successfully", message);
            }
        }

        [Fact]
        public async Task DeleteMessage_WithInvalidId_ReturnsNotFoundObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var messageId = 100;

                // Act
                var result = await controller.DeleteMessage(messageId);

                // Assert
                Assert.NotNull(result);

                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);

                var message = Assert.IsType<string>(
                    notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
                Assert.Equal("message not found", message);
            }
        }

        [Fact]
        public async Task UpdateMessage_WithValidId_AndMessageContent_ReturnsOkObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var messageId = 1;

                // Act
                var result = await controller.UpdateMessage(messageId, "Hello, World!");

                // Assert
                Assert.NotNull(result);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                var message = Assert.IsType<Message>(okResult.Value);
                Assert.Equal("Hello, World!", message.MessageContent);
            }
        }

        [Fact]
        public async Task UpdateMessage_WithValidId_AndNullMessageContent_ReturnsBadRequestObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange
                var controller = new MessageController(context);
                var messageId = 1;
                var messageContent = null as string;

                // Act
                var result = await controller.UpdateMessage(messageId, messageContent);

                // Assert
                Assert.NotNull(result);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(400, badRequestResult.StatusCode);

                var message = Assert.IsType<string>(
                    badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value));
                Assert.Equal("Message object is null", message);
            }
        }

        [Fact]
        public async Task UpdateMessage_WithInvalidId_AndMessageContent_ReturnsNotFoundObjectResult_WithMessage()
        {
            using (var context = await ArrangeTestData())
            {
                // Arrange 
                var controller = new MessageController(context);
                var messageId = 100;

                // Act
                var result = await controller.UpdateMessage(messageId, "Hello, World!");

                // Assert
                Assert.NotNull(result);

                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);

                var message = Assert.IsType<string>(
                    notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
                Assert.Equal("Message not found", message);
            }
        }
    }
}
