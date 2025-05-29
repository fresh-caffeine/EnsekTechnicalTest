namespace Ensek.Api.Models;

public class CsvParseResult<T> where T : class
{
    public List<T> Records { get; set; } = [];

    public List<string> Errors { get; set; } = [];
    
    public bool HasErrors => Errors.Count > 0;
    
    public void AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
    }
    
    public void AddRecord(T record)
    {
        Records.Add(record);
    }
}