using Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Unit.Tests.Users;
public class RefreshTokenTests
{
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenString = "test-token";
        var expiryDays = 7;

        // Act
        var token = RefreshToken.Create(userId, tokenString, expiryDays);

        // Assert
        Assert.Equal(userId, token.UserId);
        Assert.Equal(tokenString, token.Token);
        Assert.True(token.IsActive);
        Assert.False(token.IsExpired);
        // Checking if expiry is roughly 7 days from now
        Assert.True(token.ExpiresAt > DateTimeOffset.UtcNow.AddDays(6));
    }

    [Fact]
    public void IsActive_ShouldBeFalse_WhenRevoked()
    {
        // Arrange
        var token = RefreshToken.Create(Guid.NewGuid(), "token", 7);

        // Act
        token.Revoked = DateTime.UtcNow;

        // Assert
        Assert.False(token.IsActive);
    }
}