using Ensek.Api.Data;
using Ensek.Api.Endpoints;
using Ensek.Api.Services;
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
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountDbService>();
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), csvFilePath);
        
        var result = accountService.SeedAccounts(db, csvPath);
        if (result.HasErrors)
        {   
            app.Logger.LogError("Failed to seed accounts from {csvFilePath}. Errors: {Errors}", 
                csvFilePath,
                string.Join(", ", result.Errors));
        }
  
        app.Logger.LogInformation("Successfully seeded {TotalRecords} accounts from {FilePath}.", 
            result.TotalInserted,
            csvFilePath);
    }
}