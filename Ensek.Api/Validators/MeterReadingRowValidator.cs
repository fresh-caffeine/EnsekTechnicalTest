using Ensek.Api.Models;

namespace Ensek.Api.Validators;

public class MeterReadingRowValidator : IRowValidator
{
    private const int MeterMaxValue = 99999;
    
    public List<CsvRowError> Validate(MeterReading row)
    {
        List<string> errorMessageList = [];

        if (row.AccountId <= 0)
        {
            errorMessageList.Add("Invalid AccountId: " + row.AccountId);
        }
        
        if (row.ReadingValue is <= 0 or > MeterMaxValue)
        {
            errorMessageList.Add("Invalid MeterReadValue: " + row.ReadingValue);
        }
        
        if (row.ReadingDate == default || row.ReadingDate > DateTime.Today)
        {
            errorMessageList.Add("Invalid MeterReadingDateTime: " + row.ReadingDate);
        }

        return errorMessageList.Select(x => new CsvRowError(row.RowNumber, x)).ToList();
    }
}