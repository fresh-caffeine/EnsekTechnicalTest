namespace Ensek.Api.Models;

public class CsvRowError(int rowNumber, string errorMessage)
{
    public int RowNumber { get; set; } = rowNumber;
    public string ErrorMessage { get; set; } = errorMessage;
}