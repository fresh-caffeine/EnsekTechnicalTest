using Ensek.Api.Models;

namespace Ensek.Api.Services;

public interface IMeterReadingDbService
{
    Task<DbInsertResult<CsvRowError>> AddMeterReadingAsync(MeterReading meterReading);
}