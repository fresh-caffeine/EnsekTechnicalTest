using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Tests.Services;

public class MeterReadingDbServiceTests
{
    
    private Mock<ILogger<MeterReadingDbService>> _loggerMock;
    
    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<MeterReadingDbService>>();
    }
    
    [Test]
    public async Task AddMeterReadingAsync_InsertsReading_WhenAccountExistsAndNoDuplicate()
    {
        var context = CreateDbContext(nameof(AddMeterReadingAsync_InsertsReading_WhenAccountExistsAndNoDuplicate));
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        await context.SaveChangesAsync();

        var service = new MeterReadingDbService(_loggerMock.Object, context);

        var meterReading = new MeterReading
        {
            AccountId = 1,
            ReadingDate = DateTime.UtcNow,
            ReadingValue = 123,
            RowNumber = 1
        };

        var result = await service.AddMeterReadingAsync(meterReading);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalInserted, Is.EqualTo(1));
            Assert.That(result.Errors, Is.Empty);
            Assert.That(context.MeterReadings.Count(), Is.EqualTo(1));
        });
    }
    
    [Test]
    public async Task AddMeterReadingAsync_InsertsReading_WhenAccountAndOlderReadingExists()
    {
        var readingDate = DateTime.Today.AddHours(1);
        var context = CreateDbContext(nameof(AddMeterReadingAsync_InsertsReading_WhenAccountAndOlderReadingExists));
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        context.MeterReadings.Add(new MeterReading { AccountId = 1, ReadingDate = readingDate, ReadingValue = 123 });
        await context.SaveChangesAsync();

        var service = new MeterReadingDbService(_loggerMock.Object, context);

        var meterReading = new MeterReading
        {
            AccountId = 1,
            ReadingDate = readingDate.AddHours(1),
            ReadingValue = 123,
            RowNumber = 1
        };

        var result = await service.AddMeterReadingAsync(meterReading);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalInserted, Is.EqualTo(1));
            Assert.That(result.Errors, Is.Empty);
            Assert.That(context.MeterReadings.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task AddMeterReadingAsync_ReturnsError_WhenAccountIdIsMissing()
    {
        var context = CreateDbContext(nameof(AddMeterReadingAsync_ReturnsError_WhenAccountIdIsMissing));
        var service = new MeterReadingDbService(_loggerMock.Object, context);

        var meterReading = new MeterReading
        {
            AccountId = 0, // Missing account ID
            ReadingDate = DateTime.UtcNow,
            ReadingValue = 123,
            RowNumber = 1
        };

        var result = await service.AddMeterReadingAsync(meterReading);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalInserted, Is.EqualTo(0));
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.First().ErrorMessage, Does.Contain("Account with ID 0 does not exist"));
        });
    }

    [Test]
    public async Task AddMeterReadingAsync_ReturnsError_WhenDuplicateExists()
    {
        var readingDate = DateTime.Today.AddHours(1);
        var context = CreateDbContext(nameof(AddMeterReadingAsync_ReturnsError_WhenDuplicateExists));
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        context.MeterReadings.Add(new MeterReading { AccountId = 1, ReadingDate = readingDate, ReadingValue = 123 });
        await context.SaveChangesAsync();

        var service = new MeterReadingDbService(_loggerMock.Object, context);

        var meterReading = new MeterReading
        {
            AccountId = 1,
            ReadingDate = readingDate, // Future date
            ReadingValue = 123,
            RowNumber = 1
        };

        var result = await service.AddMeterReadingAsync(meterReading);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalInserted, Is.EqualTo(0));
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.First().ErrorMessage, Does.Contain("A duplicate or newer meter reading for account 1 already exists."));
        });
    }
    
    [Test]
    public async Task AddMeterReadingAsync_ReturnsError_WhenNewerReadingExists()
    {
        var readingDate = DateTime.Today.AddHours(1);
        var context = CreateDbContext(nameof(AddMeterReadingAsync_ReturnsError_WhenNewerReadingExists));
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        context.MeterReadings.Add(new MeterReading { AccountId = 1, ReadingDate = readingDate, ReadingValue = 123 });
        await context.SaveChangesAsync();

        var service = new MeterReadingDbService(_loggerMock.Object, context);

        var meterReading = new MeterReading
        {
            AccountId = 1,
            ReadingDate = readingDate.AddDays(-1), // Future date
            ReadingValue = 123,
            RowNumber = 1
        };

        var result = await service.AddMeterReadingAsync(meterReading);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalInserted, Is.EqualTo(0));
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.First().ErrorMessage, Does.Contain("A duplicate or newer meter reading for account 1 already exists."));
        });
    }
    
    [Test]
    public async Task AddMeterReadingAsync_AllowsMultipleReadingsForDifferentAccounts()
    {
        var context = CreateDbContext(nameof(AddMeterReadingAsync_AllowsMultipleReadingsForDifferentAccounts));
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        context.Accounts.Add(new Account { AccountId = 2, FirstName = "Jane", LastName = "Smith" });
        await context.SaveChangesAsync();

        var service = new MeterReadingDbService(_loggerMock.Object, context);

        var meterReading1 = new MeterReading
        {
            AccountId = 1,
            ReadingDate = DateTime.UtcNow,
            ReadingValue = 123,
            RowNumber = 1
        };

        var meterReading2 = new MeterReading
        {
            AccountId = 2,
            ReadingDate = DateTime.UtcNow,
            ReadingValue = 456,
            RowNumber = 2
        };

        var result1 = await service.AddMeterReadingAsync(meterReading1);
        var result2 = await service.AddMeterReadingAsync(meterReading2);

        Assert.Multiple(() =>
        {
            Assert.That(result1.TotalInserted, Is.EqualTo(1));
            Assert.That(result1.Errors, Is.Empty);
            Assert.That(result2.TotalInserted, Is.EqualTo(1));
            Assert.That(result2.Errors, Is.Empty);
            Assert.That(context.MeterReadings.Count(), Is.EqualTo(2));
        });
    }

    private static MeterReadingsDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<MeterReadingsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new MeterReadingsDbContext(options);
    }
}