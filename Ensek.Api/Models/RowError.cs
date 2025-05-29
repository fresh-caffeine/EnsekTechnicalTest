namespace Ensek.Api.Models;

public class RowError(int rowNumber, string errorMessage)
{
    public int RowNumber { get; set; } = rowNumber;
    public string ErrorMessage { get; set; } = errorMessage;
}