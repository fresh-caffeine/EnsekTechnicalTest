using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Models;

namespace Ensek.Api.Services;

public interface IMeterReadingService
{
    public Task<IResult> ProcessMeterReadingFile(IFormFile file);
}

public class MeterReadingService(
    ILogger<MeterReadingService> logger,
    IMeterReadingDbService meterReadingDbService
    ): IMeterReadingService
{
    private const int MeterMaxValue = 99999;
    
    public async Task<IResult> ProcessMeterReadingFile(IFormFile file)
    {
        // Read the CSV file
        if (!IsFileFormatValid(file, out var badRequest) && badRequest != null)
        {
            return badRequest;
        }

        logger.LogInformation("Processing file: {FileName}", file.FileName);
        try
        {
            var processedRows = 0;
            List<RowError> failedRows = [];
            List<MeterReadingCsvRow> meterReadings = [];
            
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });
            csv.Context.RegisterClassMap<MeterReadingCsvRowMap>();

            // Read the CSV records     
            var rows =  csv.GetRecordsAsync<MeterReadingCsvRow>();
            
            await foreach (var record in rows)
            {
                record.Rownumber = csv.Context.Parser!.Row;
                processedRows++;
                logger.LogInformation("Processing record: {AccountId}", record.AccountId);
                var rowNumber = csv.Context.Parser?.Row ?? 0;
                if (!IsRowValid(record, out var errorMessages))
                {
                    logger.LogWarning("Invalid record: {ErrorMessage}", errorMessages);
                    failedRows.AddRange(errorMessages);
                    continue; // Skip to the next record
                }
                meterReadings.Add(record);
            }
            
            logger.LogInformation("File processed successfully: {FileName}", file.FileName);
            
            var insertResult = await meterReadingDbService.AddMeterReadingsAsync(meterReadings);
            
            if (insertResult.HasErrors)
            {
                logger.LogWarning("Some records failed to insert: {ErrorCount}", insertResult.Errors.Count);
                failedRows.AddRange(insertResult.Errors);
            }
            else
            {
                logger.LogInformation("All records inserted successfully.");
            }
            
            var failedRowsCount = failedRows.DistinctBy(x => x.RowNumber).Count();
            
            return Results.Ok(new
            {
                Message = "File processed successfully",
                RecordCount = processedRows,
                SucccessRecordCount = processedRows - failedRowsCount,
                FailedRecordCount = failedRowsCount,
                Errors = failedRows.OrderBy(x => x.RowNumber)
            });
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Results.BadRequest("Error processing file: " + e.Message);
        }
    }

    private static bool IsRowValid(MeterReadingCsvRow row, out List<RowError> errorMessages)
    {
        List<string> errorMessageList = [];

        if (row.AccountId <= 0)
        {
            errorMessageList.Add("Invalid AccountId: " + row.AccountId);
        }
        
        if (row.MeterReadValue is <= 0 or > MeterMaxValue)
        {
            errorMessageList.Add("Invalid MeterReadValue: " + row.MeterReadValue);
        }
        
        if (row.MeterReadingDateTime == default)
        {
            errorMessageList.Add("Invalid MeterReadingDateTime: " + row.MeterReadingDateTime);
        }
        
        if (row.MeterReadingDateTime > DateTime.Today)
        {
            errorMessageList.Add("Invalid MeterReadingDateTime: " + row.MeterReadingDateTime);
        }

        errorMessages = errorMessageList.Select(x => new RowError(row.Rownumber, x)).ToList();
        return errorMessageList.Count == 0;
    }

    private bool IsFileFormatValid(IFormFile? file, out IResult? badRequest)
    {
        if (file == null || file.Length == 0)
        {
            logger.LogError("No file uploaded or file is empty.");
            badRequest = Results.BadRequest("No file uploaded or file is empty.");
            return false;
        }

        // Convert the file to a stream and read it using CsvHelper
        if (file.ContentType != "text/csv")
        {
            logger.LogError("Invalid file type. Only CSV files are allowed.");
            badRequest = Results.BadRequest("Invalid file type. Only CSV files are allowed.");
            return false;
        }

        badRequest = null;
        return true;
    }
    


    
    
    
}