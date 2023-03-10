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

        [Fact]
        public async Task GetAllMessages_ReturnsOkObjectResult_WithListOfMessages()
        {
            // Arrange
            using (var context = new ChatDB(options))
            {
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
            }

            using (var context = new ChatDB(options))
            {
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
    }
}