namespace Web.Infrastructure.options;

public class MyRateLimiterOptions
{
    public int Window { get; set; } = 100;
    public int PermitLimit { get; set; } = 100;
    public int TokenLimit { get; set; } = 100;
    public int ReplenishmentPeriod { get; set; } = 100;
    public int TokensPerPeriod { get; set; } = 100;
    public int QueueLimit { get; set; } = 2;
    public bool AutoReplenishment { get; internal set; }
    public int SegmentsPerWindow { get; internal set; }
}
