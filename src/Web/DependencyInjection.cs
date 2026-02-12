using Application.Identity.Services;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi;
using System.Globalization;
using System.Threading.RateLimiting;
using Web.Infrastructure;
using Web.Infrastructure.options;
using Web.Services;

namespace Web;
public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        AddSwagger(builder);
        AddRateLimiter(builder);
        AddServices(builder);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

    }

    private static void AddSwagger(IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
            {
                Title = " API Title",
                Version = "v1",
                Description = "A simple API for ",
                Contact = new Microsoft.OpenApi.OpenApiContact
                {
                    Name = "Mahmoud Sayed",
                    Email = "mahmoudsayed1332002@gmail.com",
                }
            });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            };
            opt.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
            });

        });

    }
    private static void AddRateLimiter(IHostApplicationBuilder builder)
    {
        builder.Services.Configure<MyRateLimiterOptions>(builder.Configuration.GetSection("MyRateLimiterOptions"));

        var myOptions = new MyRateLimiterOptions();
        builder.Configuration.GetSection(nameof(MyRateLimiterOptions)).Bind(myOptions);

        builder.Services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.OnRejected = (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
                    .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                    .LogWarning("OnRejected: {GetUserEndPoint}",
                    $"User {context.HttpContext.User.Identity?.Name ?? "Anonymous"} endpoint:{context.HttpContext.Request.Path}");

                return new ValueTask();
            };
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            limiterOptions.AddPolicy(policyName: RateLimiterPolicies.MainPolicy, partitioner: httpContext =>
            {
                string identity = httpContext.User.Identity?.Name ?? string.Empty;
                if (!StringValues.IsNullOrEmpty(identity))
                {
                    return RateLimitPartition.GetTokenBucketLimiter(identity, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = myOptions.TokenLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = myOptions.QueueLimit,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod),
                            TokensPerPeriod = myOptions.TokensPerPeriod,
                            AutoReplenishment = myOptions.AutoReplenishment
                        });
                }

                return RateLimitPartition.GetTokenBucketLimiter("Anon", _ =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = myOptions.TokenLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod),
                        TokensPerPeriod = myOptions.TokensPerPeriod,
                        AutoReplenishment = false
                    });
            });
        });
    }
    private static void AddServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddHttpContextAccessor();
    }

}