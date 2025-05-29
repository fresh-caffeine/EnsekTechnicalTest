using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Mappers;
using Ensek.Api.Models;

namespace Ensek.Api.Services;

public class MeterReadingCsvParser(
    ILogger<MeterReadingCsvParser> logger
    ): ICsvParser
{
    public async Task<CsvParseResult<MeterReading>> ParseAsync(IFormFile file)
    {
        var result = new CsvParseResult<MeterReading>();

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });
            csv.Context.RegisterClassMap<MeterReadingClassMap>();
            
            var rows =  csv.GetRecordsAsync<MeterReading>();
        
            await foreach (var record in rows)
            {
                record.RowNumber = csv.Context.Parser!.Row;
                result.AddRecord(record);
            }

            return result;    
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error parsing CSV file: {Message}", e.Message);
            result.AddError($"Error parsing CSV file: {e.Message}");
            
            return result;
        }
    }
}