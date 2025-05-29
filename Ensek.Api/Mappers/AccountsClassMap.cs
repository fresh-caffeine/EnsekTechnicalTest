using CsvHelper.Configuration;
using Ensek.Api.Models;

namespace Ensek.Api.Mappers;

public sealed class AccountsClassMap: ClassMap<Account>
{
    public AccountsClassMap()
    {
        Map(m => m.AccountId).Name("AccountId");
        Map(m => m.FirstName).Name("FirstName");
        Map(m => m.LastName).Name("LastName");
        Map(m => m.MeterReadings).Ignore(); // Ignore MeterReadings as they are not part of the CSV
    }
}