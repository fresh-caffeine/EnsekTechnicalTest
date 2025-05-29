namespace Ensek.Api.Models;

public class MeterReadingCsvRow
{
    public int AccountId { get; set; }
    public DateTime MeterReadingDateTime { get; set; }
    public decimal MeterReadValue { get; set; }
    
    public int Rownumber { get; set; }
}