namespace Ensek.Api.Models;

public class MeterReadingInsertResult
{
    public List<RowError> Errors { get; set; } = new();
    public int TotalInserted { get; set; }
    public int TotalFailed { get; set; }

    public bool HasErrors => Errors.Count > 0;

    public void AddError(int rowNumber, string errorMessage)
    {
        Errors.Add(new RowError(rowNumber, errorMessage));
        TotalFailed++;
    }

    public void IncrementInserted()
    {
        TotalInserted++;
    }
}