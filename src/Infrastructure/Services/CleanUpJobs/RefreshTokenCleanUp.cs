using Application.Identity.Services;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.Identity.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.CleanUpJobs;
internal class RefreshTokenCleanUp
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefreshTokenCleanUp> _logger;
    public RefreshTokenCleanUp(ApplicationDbContext context, ILogger<RefreshTokenCleanUp> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void WeeklyCleanUpRefreshToken()
    {
        RecurringJob.AddOrUpdate(
            "WeeklyCleanUpRefreshToken",
            () => RemoveOldRefresh(),
            Cron.Weekly(DayOfWeek.Sunday, 4)); //sunday at 04:00 AM utc
    }

    [Queue("cleanup")]
    public async Task RemoveOldRefresh()
    {
        var deletedCount = await _context.RefreshTokens
            .Where(r => r.ExpiresAt < DateTimeOffset.UtcNow)
            .ExecuteDeleteAsync();

        _logger.LogInformation($"Deleted {deletedCount} expired refresh tokens");
    }
}
