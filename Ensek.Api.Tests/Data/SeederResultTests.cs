using Ensek.Api.Data;
using Ensek.Api.Models;

namespace Ensek.Api.Tests.Data;

public class SeederResultTests
{
    [Test]
    public void SuccessResult_WithTotalRecords_SetsIsSuccessfulTrueWithTotalRecords()
    {
        // Arrange & Act
        var result = SeederResult.SuccessResult(10);
            
        // Assert
        Assert.Multiple(() => {
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.TotalRecords, Is.EqualTo(10));
            Assert.That(result.Errors, Is.Empty);
        });
    }

    [Test]
    public void FailureResult_WithErrorList_SetsIsSuccessfulFalseWithErrorList()
    {
        // Arrange
        var errors = new List<string> { "Error1", "Error2" };
            
        // Act
        var result = SeederResult.FailureResult(errors);
            
        // Assert
        Assert.Multiple(() => {
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.TotalRecords, Is.EqualTo(0));
            Assert.That(result.Errors, Is.EqualTo(errors));
        });
    }

    [Test]
    public void FailureResult_WithString_SetsIsSuccessfulFalseWithErrorList()
    {
        // Arrange & Act
        var result = SeederResult.FailureResult("Only error");
            
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo("Only error"));
            Assert.That(result.TotalRecords, Is.EqualTo(0));
        });
    }
}