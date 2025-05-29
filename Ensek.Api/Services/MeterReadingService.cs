using Ensek.Api.Models;
using Ensek.Api.Validators;

namespace Ensek.Api.Services;

public class MeterReadingService(
    ILogger<MeterReadingService> logger,
    IFileValidator fileValidator,
    ICsvParser csvParser,
    IRowValidator rowValidator,
    IMeterReadingDbService meterReadingDbService
    ): IMeterReadingService
{
    
    public async Task<IResult> ProcessMeterReadingFile(IFormFile? file)
    {
        // Read the CSV file
        if (!fileValidator.IsValid(file, out var errorList))
        {
            var errorMessages = errorList ?? [];
            logger.LogError("Error processing file: {Errors}", string.Join(", ", errorMessages));  
            return BadRequestResponse(errorMessages);
        }

        try
        {
            var parseResult = await csvParser.ParseAsync(file!);
            
            if (parseResult.HasErrors)
            {
                logger.LogError("Error parsing file: {Errors}", string.Join(", ", parseResult.Errors));
                return BadRequestResponse(parseResult.Errors);
            }
            
            List<CsvRowError> failedRows = [];
            
            foreach (var record in parseResult.Records)
            {
                var errors = rowValidator.Validate(record);
                if (errors.Count != 0)
                {
                    failedRows.AddRange(errors);
                    continue;
                }
                
                var insertResult = await meterReadingDbService.AddMeterReadingAsync(record);

                if (insertResult.HasErrors)
                {
                    failedRows.AddRange(insertResult.Errors);
                }
            }
            
            logger.LogInformation("Parsed {TotalRows} rows, {InvalidRows} invalid",
                parseResult.Records.Count, failedRows.Count);
            
            var failedRowsCount = failedRows.DistinctBy(x => x.RowNumber).Count();

            return Results.Ok(new
            {
                Message = "File processed successfully",
                RecordCount = parseResult.Records.Count,
                SuccessRecordCount = parseResult.Records.Count - failedRowsCount,
                FailedRecordCount = failedRowsCount,
                Errors = failedRows.OrderBy(x => x.RowNumber)
            });
            
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing file");
            return BadRequestResponse([e.Message]);
        }
    }

    private static IResult BadRequestResponse(IEnumerable<string> badRequest)
    {
        return Results.BadRequest(new
        {
            Message = "Error processing file",
            Errors = badRequest
        });
    }
}