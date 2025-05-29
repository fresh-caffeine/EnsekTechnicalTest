using Ensek.Api.Models;

namespace Ensek.Api.Services;

public interface ICsvParser
{
    Task<CsvParseResult<MeterReading>> ParseAsync(IFormFile file);
}