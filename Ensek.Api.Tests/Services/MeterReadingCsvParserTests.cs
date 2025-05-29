namespace Ensek.Api.Tests.Services;

public class MeterReadingCsvParserTests
{
    [Test]
    public async Task ParseAsync_ReturnsRecords_WhenCsvIsValid()
    {
        var logger = new Mock<ILogger<MeterReadingCsvParser>>();
        const string csvContent = "AccountId,MeterReadingDateTime,MeterReadValue\n1234,01/01/2023 09:24,1000\n";
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var parser = new MeterReadingCsvParser(logger.Object);

        var result = await parser.ParseAsync(fileMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(result.Records.Count, Is.EqualTo(1));
            Assert.That(result.Errors, Is.Empty);
        });
    }

    [Test]
    public async Task ParseAsync_ReturnsError_WhenCsvIsMalformed()
    {
        var logger = new Mock<ILogger<MeterReadingCsvParser>>();
        const string csvContent = "AccountId,MeterReadingDateTime,MeterReadValue\nBADROW";
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var parser = new MeterReadingCsvParser(logger.Object);

        var result = await parser.ParseAsync(fileMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(result.Records, Is.Empty);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.First(), Does.Contain("Error parsing CSV file"));
        });
    }

    [Test]
    public async Task ParseAsync_ReturnsEmptyResult_WhenFileIsEmpty()
    {
        var logger = new Mock<ILogger<MeterReadingCsvParser>>();
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream();
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var parser = new MeterReadingCsvParser(logger.Object);

        var result = await parser.ParseAsync(fileMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(result.Records, Is.Empty);
            Assert.That(result.Errors, Is.Empty);
        });
    }

    [Test]
    public async Task ParseAsync_ReturnsError_WhenExceptionIsThrown()
    {
        var logger = new Mock<ILogger<MeterReadingCsvParser>>();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Throws(new IOException("Stream error"));

        var parser = new MeterReadingCsvParser(logger.Object);

        var result = await parser.ParseAsync(fileMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(result.Records, Is.Empty);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.First(), Does.Contain("Error parsing CSV file"));
        });
    }
}