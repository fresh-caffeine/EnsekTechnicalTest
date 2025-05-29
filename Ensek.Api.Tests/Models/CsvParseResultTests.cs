namespace Ensek.Api.Tests.Models;

public class CsvParseResultTests
{
    [Test]
    public void AddError_IncreasesErrorCountAndHasErrorsBecomesTrue()
    {
        var result = new CsvParseResult<string>();
        result.AddError("Parse error");

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.HasErrors, Is.True);
            Assert.That(result.Errors[0], Is.EqualTo("Parse error"));
        });
    }

    [Test]
    public void AddRecord_IncreasesRecordsCount()
    {
        var result = new CsvParseResult<string>();
        const string record = "Record 1";
        result.AddRecord(record);

        Assert.That(result.Records, Has.Count.EqualTo(1));
        Assert.That(result.Records[0], Is.EqualTo(record));
    }

    [Test]
    public void HasErrors_ReturnsFalse_WhenNoErrorsAdded()
    {
        var result = new CsvParseResult<string>();

        Assert.That(result.HasErrors, Is.False);
    }

    [Test]
    public void AddError_CanAddMultipleErrors()
    {
        var result = new CsvParseResult<string>();
        result.AddError("Error 1");
        result.AddError("Error 2");

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Has.Count.EqualTo(2));
            Assert.That(result.Errors, Does.Contain("Error 1"));
            Assert.That(result.Errors, Does.Contain("Error 2"));
        });
    }
}