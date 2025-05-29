using System.Globalization;
using CsvHelper.Configuration;
using Ensek.Api.Models;

namespace Ensek.Api.Services;

public sealed class MeterReadingCsvRowMap : ClassMap<MeterReadingCsvRow>
{
    public MeterReadingCsvRowMap()
    {
        Map(m => m.MeterReadingDateTime)
            .Name("MeterReadingDateTime")
            .TypeConverterOption
            .DateTimeStyles(DateTimeStyles.AssumeLocal)
            .TypeConverterOption
            .Format("dd/MM/yyyy HH:mm");
        Map(m => m.AccountId).Name("AccountId");
        Map(m => m.MeterReadValue).Name("MeterReadValue");
        Map(m => m.Rownumber).Ignore();
    }
}