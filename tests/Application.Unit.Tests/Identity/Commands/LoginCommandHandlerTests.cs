using Application.Common.Models;
using Application.Identity.Commands;
using Application.Identity.Dtos;
using Application.Identity.Services;
using Moq;

namespace Application.Unit.Tests.Identity.Commands;
public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithEmailAndPassword_ReturnsLoginResponseModel()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "Password123!"
        };
        var identityServiceMock = new Mock<IIdentityService>();
        identityServiceMock.Setup(service =>
            service.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(new LoginResponseModel
            {
                Token = It.IsAny<string>(),
                RefreshToken = It.IsAny<string>(),
                TokenExpiryInMinutes = It.IsAny<int>(),
                UserId = It.IsAny<string>(),
                Roles = It.IsAny<ICollection<string>>(),
            });
        var handler = new LoginCommandHandler(identityServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponseModel>(result);
    }
}
