namespace Ensek.Api.Models;

public class Account
{
    public int AccountId { get; set; } // primary key
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public List<MeterReading> MeterReadings { get; set; } = [];
}