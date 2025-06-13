using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Data;
using Ensek.Api.Mappers;
using Ensek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Services;

public class AccountDbService(
    ILogger<AccountDbService> logger,
    MeterReadingsDbContext context
    ): IAccountDbService 
{
    public DbInsertResult<string> SeedAccounts(string csvPath)
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
    
    public async Task<List<Account>> GetAccounts()
    {
        return await context.Accounts
            .AsNoTracking()
            .Include(a => a.MeterReadings)
            .ToListAsync();
    }

    public async Task<Account?> GetAccountById(int accountId)
    {
        if (accountId <= 0)
        {
            logger.LogWarning("Invalid account ID: {AccountId}", accountId);
            return null;
        }
        
        return await context.Accounts
            .AsNoTracking()
            .Include(a => a.MeterReadings)
            .FirstOrDefaultAsync(a => a.AccountId == accountId);
    }
}