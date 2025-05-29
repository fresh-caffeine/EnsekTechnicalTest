using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Data;
using Ensek.Api.Mappers;
using Ensek.Api.Models;

namespace Ensek.Api.Services;

public interface IAccountDbService
{
    DbInsertResult<string> SeedAccounts(MeterReadingsDbContext context, string csvPath);
}

public class AccountDbService(
    ILogger<AccountDbService> logger
    ): IAccountDbService 
{
    public DbInsertResult<string> SeedAccounts(MeterReadingsDbContext context, string csvPath)
    {
        var result = new DbInsertResult<string>();
        
        
        
        if (!File.Exists(csvPath)) 
        {
            logger.LogError("CSV file not found at path: {csvPath}", csvPath);
            result.AddError($"CSV file not found at path: {csvPath}");
            return result;
        }
        
        if (context.Accounts.Any())
        {
            logger.LogInformation("Accounts already seeded. Skipping seeding process.");
            
            return result;
        }

        try
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            csv.Context.RegisterClassMap<AccountsClassMap>();
            
            var accounts = csv.GetRecords<Account>().ToList();
            
            context.Accounts.AddRange(accounts);
            context.SaveChanges();

            result.IncrementInserted(accounts.Count);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while seeding accounts from CSV.");
            result.AddError($"Error occurred while seeding accounts: {e.Message}");
            return result;
        }
    }
}