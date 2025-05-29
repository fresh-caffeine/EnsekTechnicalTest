using Ensek.Api.Models;

namespace Ensek.Api.Services;

public interface IMeterReadingDbService
{
    Task<MeterReadingInsertResult> AddMeterReadingsAsync(List<MeterReadingCsvRow> meterReadings);
}

public class MeterReadingDbService: IMeterReadingDbService
{
    public Task<MeterReadingInsertResult> AddMeterReadingsAsync(List<MeterReadingCsvRow> meterReadings)
    {
        
        
        // Simulate database insertion logic
        var result = new MeterReadingInsertResult();
        
        foreach (var reading in meterReadings)
        {
            // Simulate validation and insertion
            if (reading.MeterReadValue is < 0 or > 999)
            {
                result.AddError(reading.Rownumber, 
                    $"Invalid MeterReadValue: {reading.MeterReadValue}. Must be between 0 and 999.");
            }
            else
            {
                result.IncrementInserted();
            }
        }
        
        return Task.FromResult(result);
        
        
    }
}