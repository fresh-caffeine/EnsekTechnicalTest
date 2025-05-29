using Ensek.Api.Validators;

namespace Ensek.Api.Tests.Validators;

public class MeterReadingFileValidatorTests
{
    [Test]
    public void IsValid_ReturnsTrue_WhenFileIsCsvAndNotEmpty()
    {
        var logger = new Mock<ILogger<MeterReadingFileValidator>>();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.ContentType).Returns("text/csv");

        var validator = new MeterReadingFileValidator(logger.Object);

        var result = validator.IsValid(fileMock.Object, out var errors);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(errors, Is.Empty);
        });
    }

    [Test]
    public void IsValid_ReturnsFalse_WhenFileIsNull()
    {
        var logger = new Mock<ILogger<MeterReadingFileValidator>>();
        var validator = new MeterReadingFileValidator(logger.Object);

        var result = validator.IsValid(null, out var errors);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(errors, Has.One.Items);
            Assert.That(errors[0], Does.Contain("No file uploaded or file is empty"));
        });
    }

    [Test]
    public void IsValid_ReturnsFalse_WhenFileIsEmpty()
    {
        var logger = new Mock<ILogger<MeterReadingFileValidator>>();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);
        fileMock.Setup(f => f.ContentType).Returns("text/csv");

        var validator = new MeterReadingFileValidator(logger.Object);

        var result = validator.IsValid(fileMock.Object, out var errors);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(errors, Has.One.Items);
            Assert.That(errors[0], Does.Contain("No file uploaded or file is empty"));
        });
    }

    [Test]
    public void IsValid_ReturnsFalse_WhenFileIsNotCsv()
    {
        var logger = new Mock<ILogger<MeterReadingFileValidator>>();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");

        var validator = new MeterReadingFileValidator(logger.Object);

        var result = validator.IsValid(fileMock.Object, out var errors);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(errors, Has.One.Items);

            Assert.That(errors[0], Does.Contain("Invalid file type. Only CSV files are allowed"));
        });
    }
}