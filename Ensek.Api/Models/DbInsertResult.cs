namespace Ensek.Api.Models;

public class DbInsertResult<T> where T : class
{
    public List<T> Errors { get; set; } = [];
    public int TotalInserted { get; set; }
    private int TotalFailed { get; set; }

    public bool HasErrors => Errors.Count > 0;

    public void AddError(T error)
    {
        Errors.Add(error);
        TotalFailed++;
    }

    public void IncrementInserted(int incrementAmount = 1)
    {
        TotalInserted += incrementAmount;
    }
}