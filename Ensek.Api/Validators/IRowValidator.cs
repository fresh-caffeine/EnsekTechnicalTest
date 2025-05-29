using Ensek.Api.Models;

namespace Ensek.Api.Validators;

public interface IRowValidator
{
    List<CsvRowError> Validate(MeterReading row);
}