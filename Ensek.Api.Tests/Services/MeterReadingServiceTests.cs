using Ensek.Api.Validators;

namespace Ensek.Api.Tests.Services;

public class MeterReadingServiceTests
{
    private Mock<IFileValidator> _fileValidator;
    private Mock<ICsvParser> _csvParser;
    private Mock<IRowValidator> _rowValidator;
    private Mock<IMeterReadingDbService> _meterReadingDbService;
    private Mock<ILogger<MeterReadingService>> _logger;

    [SetUp]
    public void SetUp()
    {
        _fileValidator = new Mock<IFileValidator>();
        _csvParser = new Mock<ICsvParser>();
        _rowValidator = new Mock<IRowValidator>();
        _meterReadingDbService = new Mock<IMeterReadingDbService>();
        _logger = new Mock<ILogger<MeterReadingService>>();
    }

    [Test]
    public async Task ProcessMeterReadingFile_ReturnsOk_WhenAllRowsValidAndInserted()
    {
        var file = Mock.Of<IFormFile>();
        var row = new MeterReading();
        _fileValidator.Setup(f => f.IsValid(file, out It.Ref<List<string>>.IsAny))
            .Returns(true);
        _csvParser.Setup(p => p.ParseAsync(file))
            .ReturnsAsync(new CsvParseResult<MeterReading>
            {
                Records = [row]
            });
        _rowValidator.Setup(v => v.Validate(row)).Returns(new List<CsvRowError>());
        _meterReadingDbService.Setup(d => d.AddMeterReadingAsync(row)).ReturnsAsync(new DbInsertResult<CsvRowError>());

        var service = CreateService();
        var result = await service.ProcessMeterReadingFile(file);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.StatusCode(), Is.EqualTo(StatusCodes.Status200OK));
        });
        _meterReadingDbService.Verify(d => d.AddMeterReadingAsync(row), Times.Once);
    }

    [Test]
    public async Task ProcessMeterReadingFile_ReturnsOk_WhenSomeRowsFailValidation()
    {
        var file = Mock.Of<IFormFile>();
        var row = new MeterReading();
        var error = new CsvRowError(1, "Invalid");

        _fileValidator.Setup(f => f.IsValid(file, out It.Ref<List<string>>.IsAny))
            .Returns(true);
        _csvParser.Setup(p => p.ParseAsync(file))
            .ReturnsAsync(new CsvParseResult<MeterReading>
            {
                Records = [row]
            });
        _rowValidator.Setup(v => v.Validate(row))
            .Returns([error]);

        var service = CreateService();
        var result = await service.ProcessMeterReadingFile(file);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.StatusCode(), Is.EqualTo(StatusCodes.Status200OK));
        });
        _meterReadingDbService.Verify(d => d.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Never);
    }

    [Test]
    public async Task ProcessMeterReadingFile_ReturnsOk_WhenInsertHasErrors()
    {
        var file = Mock.Of<IFormFile>();
        var row = new MeterReading();
        var error = new CsvRowError(1, "Insert error");
        _fileValidator.Setup(f => f.IsValid(file, out It.Ref<List<string>>.IsAny))
            .Returns(true);
        _csvParser.Setup(p => p.ParseAsync(file))
            .ReturnsAsync(new CsvParseResult<MeterReading>
            {
                Records = [row]
            });
        _rowValidator.Setup(v => v.Validate(row))
            .Returns([]);
        _meterReadingDbService.Setup(d => d.AddMeterReadingAsync(row))
            .ReturnsAsync(new DbInsertResult<CsvRowError>
            {
                Errors = [error]
            });

        var service = CreateService();
        var result = await service.ProcessMeterReadingFile(file);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.StatusCode(), Is.EqualTo(StatusCodes.Status200OK));
        });
        _meterReadingDbService.Verify(d => d.AddMeterReadingAsync(row), Times.Once);
    }
    
    
    [Test]
    public async Task ProcessMeterReadingFile_ReturnsBadRequest_WhenFileIsNullOrInvalid()
    {
        IFormFile? file = null;
        _fileValidator.Setup(f => f.IsValid(file, out It.Ref<List<string>>.IsAny))
            .Returns(false);

        var service = CreateService();
        var result = await service.ProcessMeterReadingFile(file);

        Assert.That(result, Is.InstanceOf<IResult>());
        _fileValidator.Verify(f => f.IsValid(file, out It.Ref<List<string>?>.IsAny), Times.Once);
    }

    [Test]
    public async Task ProcessMeterReadingFile_ReturnsBadRequest_WhenCsvParseHasErrors()
    {
        var file = Mock.Of<IFormFile>();
        _fileValidator.Setup(f => f.IsValid(file, out It.Ref<List<string>>.IsAny))
            .Returns(true);
        _csvParser.Setup(p => p.ParseAsync(file))
            .ReturnsAsync(new CsvParseResult<MeterReading>
            {
                Errors = ["Parse error"]
            });

        var service = CreateService();
        var result = await service.ProcessMeterReadingFile(file);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.StatusCode(), Is.EqualTo(StatusCodes.Status400BadRequest));
        });
        _csvParser.Verify(p => p.ParseAsync(file), Times.Once);
    }


    [Test]
    public async Task ProcessMeterReadingFile_ReturnsBadRequest_WhenExceptionThrown()
    {
        var file = Mock.Of<IFormFile>();
        _fileValidator.Setup(f => f.IsValid(file, out It.Ref<List<string>>.IsAny))
            .Returns(true);
        _csvParser.Setup(p => p.ParseAsync(file))
            .ThrowsAsync(new Exception("Unexpected"));

        var service = CreateService();
        var result = await service.ProcessMeterReadingFile(file);


        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(result.StatusCode(), Is.EqualTo(StatusCodes.Status400BadRequest));
        });
    }

    private MeterReadingService CreateService()
    {
        return new MeterReadingService(
            _logger.Object,
            _fileValidator.Object,
            _csvParser.Object,
            _rowValidator.Object,
            _meterReadingDbService.Object
        );
    }
}