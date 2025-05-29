using Ensek.Api.Data;
using Ensek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Services;

public class MeterReadingDbService(
    ILogger<MeterReadingDbService> logger,
    MeterReadingsDbContext context): IMeterReadingDbService
{
    public async Task<DbInsertResult<CsvRowError>> AddMeterReadingAsync(MeterReading meterReading)
    {
        var result = new DbInsertResult<CsvRowError>();

        try
        {
            var accountExists = await context.Accounts
                .AnyAsync(a => a.AccountId == meterReading.AccountId);
            if (!accountExists)
            {
                result.AddError(new CsvRowError(meterReading.RowNumber, $"Account with ID {meterReading.AccountId} does not exist."));
                return result;
            }
            
            var duplicateOrNewerExists = await context.MeterReadings
                .AnyAsync(mr => mr.AccountId == meterReading.AccountId && 
                                mr.ReadingDate >= meterReading.ReadingDate);
            if (duplicateOrNewerExists)
            {
                result.AddError(new CsvRowError(meterReading.RowNumber, 
                    $"A duplicate or newer meter reading for account {meterReading.AccountId} already exists."));
                return result;
            }
            
            var entity = new MeterReading
            {
                AccountId = meterReading.AccountId,
                ReadingValue = meterReading.ReadingValue,
                ReadingDate = meterReading.ReadingDate
            };
            
            await context.MeterReadings.AddAsync(entity);
            await context.SaveChangesAsync();
            
            result.IncrementInserted();
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error inserting meter reading for account {AccountId} on {ReadingDate:g}",
                meterReading.AccountId, meterReading.ReadingDate);
            result.AddError(new CsvRowError(meterReading.RowNumber, 
            $"Error inserting meter reading for account {meterReading.AccountId} on {meterReading.ReadingDate:g}."));
            
            return result;
        }
    }
}