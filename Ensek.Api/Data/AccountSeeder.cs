using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Models;

namespace Ensek.Api.Data;

public static class AccountSeeder
{
    public static SeederResult Seed(MeterReadingsDbContext context, string csvPath, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(AccountSeeder));
        
        if (!File.Exists(csvPath)) 
        {
            logger.LogError("CSV file not found at path: {csvPath}", csvPath);
            return SeederResult.FailureResult($"CSV file not found at path: {csvPath}");
        }
        
        if (context.Accounts.Any())
        {
            logger.LogInformation("Accounts already seeded. Skipping seeding process.");
            return SeederResult.SuccessResult(context.Accounts.Count());
        }

        try
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            var csvRecords = csv.GetRecords<AccountCsvRow>();
            var accounts = csvRecords.Select(record => new Account //TODO: Use Mapper
            {
                AccountId = record.AccountId,
                FirstName = record.FirstName,
                LastName = record.LastName
            }).ToList();
        
            context.Accounts.AddRange(accounts);
            context.SaveChanges();

            return SeederResult.SuccessResult(accounts.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while seeding accounts from CSV.");
            return SeederResult.FailureResult($"Error occurred while seeding accounts: {e.Message}");
        }
    }
}