
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Data;
using Application.Features.Products.Services;
using Application.Identity.Services;
using Domain.Constants;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;
using Infrastructure.Identity.JWT;
using Infrastructure.Services.Caching;
using Infrastructure.Services.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.Text;

namespace Infrastructure
{ 
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IHostApplicationBuilder builder)
        {
            ConfigureRedisCache(builder.Services, builder.Configuration);

            ConfigureDbContext(builder.Services, builder.Configuration);

            ConfigureIdentity(builder.Services, builder.Configuration);

            // Add resilience policies
            builder.Services.AddSingleton<IAsyncPolicy>(
                Policy.Handle<Exception>().CircuitBreakerAsync(2, TimeSpan.FromMinutes(5))); // To be used with redis

            // Add policies
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy(Policies.CanManageUsers, policy => policy.RequireRole(Roles.Admin));

            // Register services
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddTransient<IIdentityService, IdentityService>();
            builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();

            builder.Services.AddScoped<IProductService, CachedProductService>();
            builder.Services.AddScoped<ProductService>();
        }
        private static void ConfigureRedisCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("Redis");
                opt.InstanceName = "App_";
            });
        }

        private static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
                options.ConfigureWarnings(warnings =>
                    warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            });

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        }

        private static void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
                o.User.RequireUniqueEmail = true;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Lockout.MaxFailedAccessAttempts = 5;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // add jwt config
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            using var scope = services.BuildServiceProvider().CreateScope();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret ?? string.Empty)),
                };
            });
        }   
    }
}
