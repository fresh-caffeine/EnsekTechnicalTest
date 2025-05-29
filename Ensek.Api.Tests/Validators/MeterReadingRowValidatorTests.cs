using Ensek.Api.Validators;

namespace Ensek.Api.Tests.Validators;

public class MeterReadingRowValidatorTests
{
    [Test]
    public void Validate_ReturnsNoErrors_WhenRowIsValid()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 1,
            ReadingValue = 12345,
            ReadingDate = DateTime.Today,
            RowNumber = 1
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void Validate_ReturnsError_WhenAccountIdIsZero()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 0,
            ReadingValue = 123,
            ReadingDate = DateTime.Today,
            RowNumber = 2
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0].ErrorMessage, Does.Contain("Invalid AccountId"));
    }

    [Test]
    public void Validate_ReturnsError_WhenMeterReadValueIsNegative()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 1,
            ReadingValue = -5,
            ReadingDate = DateTime.Today,
            RowNumber = 3
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0].ErrorMessage, Does.Contain("Invalid MeterReadValue"));
    }

    [Test]
    public void Validate_ReturnsError_WhenMeterReadValueIsZero()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 1,
            ReadingValue = 0,
            ReadingDate = DateTime.Today,
            RowNumber = 4
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0].ErrorMessage, Does.Contain("Invalid MeterReadValue"));
    }

    [Test]
    public void Validate_ReturnsError_WhenMeterReadValueExceedsMax()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 1,
            ReadingValue = 100000,
            ReadingDate = DateTime.Today,
            RowNumber = 5
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0].ErrorMessage, Does.Contain("Invalid MeterReadValue"));
    }

    [Test]
    public void Validate_ReturnsError_WhenMeterReadingDateTimeIsDefault()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 1,
            ReadingValue = 123,
            ReadingDate = default,
            RowNumber = 6
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0].ErrorMessage, Does.Contain("Invalid MeterReadingDateTime"));
    }

    [Test]
    public void Validate_ReturnsError_WhenMeterReadingDateTimeIsInFuture()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 1,
            ReadingValue = 123,
            ReadingDate = DateTime.Today.AddDays(1),
            RowNumber = 7
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0].ErrorMessage, Does.Contain("Invalid MeterReadingDateTime"));
    }

    [Test]
    public void Validate_ReturnsAllErrors_WhenAllFieldsAreInvalid()
    {
        var validator = new MeterReadingRowValidator();
        var row = new MeterReading
        {
            AccountId = 0,
            ReadingValue = -1,
            ReadingDate = default,
            RowNumber = 8
        };

        var errors = validator.Validate(row);

        Assert.That(errors, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(errors.Any(e => e.ErrorMessage.Contains("Invalid AccountId")));
            Assert.That(errors.Any(e => e.ErrorMessage.Contains("Invalid MeterReadValue")));
            Assert.That(errors.Any(e => e.ErrorMessage.Contains("Invalid MeterReadingDateTime")));
        });
    }
}