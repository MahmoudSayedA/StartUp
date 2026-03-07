namespace Domain.Entities.Users;
public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public DateTime? Revoked { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }


    public RefreshToken()
    {
        Token = string.Empty;
        ExpiresAt = DateTimeOffset.UtcNow;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    public static RefreshToken Create(
        Guid userId,
        string token,
        int expiryInDays)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var now = DateTimeOffset.UtcNow;

        return new RefreshToken
        {
            Id = Ulid.NewUlid(),
            UserId = userId,
            Token = token,
            CreatedAt = now,
            ExpiresAt = now.AddDays(expiryInDays),
        };
    }
}