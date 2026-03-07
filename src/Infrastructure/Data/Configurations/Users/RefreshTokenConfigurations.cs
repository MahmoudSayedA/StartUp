using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Users;
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.IsActive);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        builder.HasIndex(x => new { x.UserId, x.ExpiresAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

        // for clean up job
        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

    }
}