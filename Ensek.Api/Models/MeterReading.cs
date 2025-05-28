namespace Ensek.Api.Models;

public class MeterReading
{
    public int MeterReadingId { get; set; } //primary key
    public int AccountId { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal ReadingValue { get; set; }
    
    public Account Account { get; set; } = null!; // Navigation property to Account
}