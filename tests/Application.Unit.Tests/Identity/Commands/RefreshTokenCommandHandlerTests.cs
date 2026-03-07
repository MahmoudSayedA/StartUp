using Application.Common.Abstractions.Data;
using Application.Identity.Commands;
using Application.Identity.Services;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Moq;
using Moq.EntityFrameworkCore;

namespace Application.Unit.Tests.Identity.Commands;
public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<ITokenGeneratorService> _tokenGeneratorMock;
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public RefreshTokenCommandHandlerTests()
    {
        _tokenGeneratorMock = new Mock<ITokenGeneratorService>();
        _contextMock = new Mock<IApplicationDbContext>();

        // 1. Mock the UserStore
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

        // 2. Instantiate UserManager Mock with required nulls
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        // FIX: Ensure this returns an empty list instead of null by default
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        // Ensure the token generator is also ready
        _tokenGeneratorMock.Setup(x => x.GenerateJwtToken(It.IsAny<ApplicationUser>(), It.IsAny<List<string>>()))
            .Returns("new-access-token");
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewLoginResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingToken = "valid-refresh-token";
        var user = new ApplicationUser { Id = userId, UserName = "testuser" };
        var refreshToken = RefreshToken.Create(userId, existingToken, 7);

        // Mock DbSet
        var tokens = new List<RefreshToken> { refreshToken }.AsQueryable();
        var users = new List<ApplicationUser> { user }.AsQueryable();

        _contextMock.Setup(db => db.Set<RefreshToken>()).ReturnsDbSet(tokens);
        _contextMock.Setup(db => db.Set<ApplicationUser>()).ReturnsDbSet(users);

        var command = new RefreshTokenCommand { RefreshToken = existingToken };
        var commandHandler = new RefreshTokenCommandHandler(_contextMock.Object, _userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act
        var result = await commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-access-token", result.Token);
        Assert.Equal(existingToken, result.RefreshToken); // Based on your current code logic
    }

    [Fact]
    public async Task Handle_ExpiredToken_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var expiredToken = RefreshToken.Create(Guid.NewGuid(), "expired", -1); // Created in the past
        var tokens = new List<RefreshToken> { expiredToken }.AsQueryable();

        _contextMock.Setup(db => db.Set<RefreshToken>()).ReturnsDbSet(tokens);

        var command = new RefreshTokenCommand { RefreshToken = "expired" };
        var commandHandler = new RefreshTokenCommandHandler(_contextMock.Object, _userManagerMock.Object, _tokenGeneratorMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            commandHandler.Handle(command, CancellationToken.None));
    }

}
