using System.Runtime.CompilerServices;
using Ensek.Api.Data;
using Ensek.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.Logger.LogInformation("Mapping endpoints...");
        app.MapMeterReadingEndpoints();
    }
    
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();
        db.Database.Migrate();
        
        return app;
    }

    public static void SeedAccountsFromCsv(this WebApplication app, string csvFilePath)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), csvFilePath);
        
        var result = AccountSeeder.Seed(db, csvPath, loggerFactory);
        if (result.IsSuccessful)
        {
            app.Logger.LogInformation("Successfully seeded {TotalRecords} accounts from {FilePath}.", 
                result.TotalRecords,
                csvFilePath);
        }
        else
        {
            app.Logger.LogError("Failed to seed accounts from {csvFilePath}. Errors: {Errors}", 
                csvFilePath,
                string.Join(", ", result.Errors));
        }
       
    }
}