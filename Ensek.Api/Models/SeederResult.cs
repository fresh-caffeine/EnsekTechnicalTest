namespace Ensek.Api.Models;

public class SeederResult
{
    public bool IsSuccessful { get; set; }
    public int TotalRecords { get; set; }
    public List<string> Errors { get; set; } = [];
    
    private SeederResult() { }

    public static SeederResult SuccessResult(int totalRecords) =>
        new()
        {
            IsSuccessful = true, 
            TotalRecords = totalRecords
        };
    
    public static SeederResult FailureResult(List<string> errors) =>
        new()
        {
            IsSuccessful = false, 
            Errors = errors
        };

    public static SeederResult FailureResult(string errors) => 
        new()
        {
            IsSuccessful = false, 
            Errors = [errors]
        };
}