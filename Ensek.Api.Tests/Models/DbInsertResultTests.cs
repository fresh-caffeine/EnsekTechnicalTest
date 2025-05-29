namespace Ensek.Api.Tests.Models;

public class DbInsertResultTests
{
    [Test]
    public void AddError_IncreasesErrorCountAndHasErrorsBecomesTrue()
    {
        var result = new DbInsertResult<string>();
        result.AddError("Some error");

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.HasErrors, Is.True);
        });
        Assert.That(result.Errors[0], Is.EqualTo("Some error"));
    }

    [Test]
    public void IncrementInserted_IncreasesTotalInserted()
    {
        var result = new DbInsertResult<string>();
        result.IncrementInserted();
        result.IncrementInserted();

        Assert.That(result.TotalInserted, Is.EqualTo(2));
    }

    [Test]
    public void HasErrors_ReturnsFalse_WhenNoErrorsAdded()
    {
        var result = new DbInsertResult<string>();

        Assert.That(result.HasErrors, Is.False);
    }

    [Test]
    public void AddError_CanAddMultipleErrors()
    {
        var result = new DbInsertResult<string>();
        result.AddError("Error 1");
        result.AddError("Error 2");

        Assert.That(result.Errors, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Does.Contain("Error 1"));
            Assert.That(result.Errors, Does.Contain("Error 2"));
        });
    }
}