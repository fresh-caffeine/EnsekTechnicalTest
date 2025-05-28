using System.Runtime.CompilerServices;
using Ensek.Api.Data;
using Ensek.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        AccountEndpoints.MapAccountEndpoints(app);
        MeterReadingEndpoints.MapAccountEndpoints(app);
    }
    
    public static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();
        db.Database.Migrate();
        
        return serviceProvider;
    }

    public static void SeedAccountsFromCsv(this IServiceProvider serviceProvider, string csvFilePath)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AccountSeeder");
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), csvFilePath);
        
        var result = AccountSeeder.Seed(db, csvPath, loggerFactory);
        if (result.IsSuccessful)
        {
            logger.LogInformation("Successfully seeded {TotalRecords} accounts from {FilePath}.", 
                result.TotalRecords,
                csvFilePath);
        }
        else
        {
            logger.LogError("Failed to seed accounts from {csvFilePath}. Errors: {Errors}", 
                csvFilePath,
                string.Join(", ", result.Errors));
        }
       
    }
}