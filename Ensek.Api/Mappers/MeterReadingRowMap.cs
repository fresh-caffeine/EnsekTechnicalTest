using System.Globalization;
using CsvHelper.Configuration;
using Ensek.Api.Models;

namespace Ensek.Api.Mappers;

public sealed class MeterReadingClassMap : ClassMap<MeterReading>
{
    public MeterReadingClassMap()
    {
        Map(m => m.ReadingDate)
            .Name("MeterReadingDateTime")
            .TypeConverterOption
            .DateTimeStyles(DateTimeStyles.AssumeLocal)
            .TypeConverterOption
            .Format("dd/MM/yyyy HH:mm");
        Map(m => m.AccountId).Name("AccountId");
        Map(m => m.ReadingValue).Name("MeterReadValue");
        Map(m => m.RowNumber).Ignore();
    }
}