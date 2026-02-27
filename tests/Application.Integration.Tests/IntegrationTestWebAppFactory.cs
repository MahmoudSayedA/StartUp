using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace Application.Integration.Tests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new 
        MsSqlBuilder(image: "mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("P@ssw0rd")

        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {

            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            string masterConn = _dbContainer.GetConnectionString();
            string connectionString = new SqlConnectionStringBuilder(masterConn)
            {
                InitialCatalog = "StartUpTests",
                TrustServerCertificate = true
            }.ConnectionString;

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            // update hangfire configuration
            services.RemoveAll<JobStorage>();
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });
        });

        builder.UseEnvironment("Testing");
    }
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // نعمل الـ DB يدويًا + migrations
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // نخلق الـ database لو مش موجودة
        await dbContext.Database.EnsureCreatedAsync(); // أو MigrateAsync لو عايز migrations كاملة

        // أفضل: نعمل migrations كاملة
        // await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
