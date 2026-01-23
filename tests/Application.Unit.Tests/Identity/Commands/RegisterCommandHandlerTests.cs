using Application.Common.Models;
using Application.Identity.Commands;
using Application.Identity.Dtos;
using Application.Identity.Services;
using MediatR;
using MediatR.NotificationPublishers;
using Moq;

namespace Application.Unit.Tests.Identity.Commands;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRegisterCommand_ReturnsUserId()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var identityServiceMock = new Mock<IIdentityService>();
        identityServiceMock.Setup(service =>
            service.RegisterAsync(It.IsAny<RegisterDto>()))
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

        var publisherMock = new Mock<IPublisher>();
        publisherMock.Setup(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RegisterCommandHandler(identityServiceMock.Object, publisherMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        identityServiceMock.Verify(s => s.RegisterAsync(It.Is<RegisterDto>(dto =>
            dto.Username == command.Username &&
            dto.Email == command.Email &&
            dto.Password == command.Password
        )), Times.Once);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }
}