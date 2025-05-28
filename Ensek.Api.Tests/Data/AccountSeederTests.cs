using Moq;
using Microsoft.Extensions.Logging;
using Ensek.Api.Data;
using Ensek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Tests.Data;

public class AccountSeederTests
{
    private Mock<ILogger> _loggerMock;
    private Mock<ILoggerFactory> _loggerFactoryMock;
    private List<string> _tempFiles;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerFactoryMock
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(_loggerMock.Object);

        _tempFiles = [];
    }
    
    [TearDown]
    public void TearDown()
    {
        foreach (var file in _tempFiles.Where(File.Exists))
        {
            File.Delete(file);
        }
    }
    
    [Test]
    public void Seed_WhenCsvFileDoesNotExist_ReturnsFailureResult()
    {
        var context = CreateDbContext("NonExistentDb");
        var result = AccountSeeder.Seed(context, "nonexistent.csv", _loggerFactoryMock.Object);
      
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.Errors[0], Does.Contain("CSV file not found"));
        });
    }

    [Test]
    public void Seed_WhenAccountsAlreadyExist_ReturnsSuccessResultWithCount()
    {
        var context = CreateDbContext("TestJohn");
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        context.SaveChanges();
        
        var result = AccountSeeder.Seed(context, Path.GetTempFileName(), _loggerFactoryMock.Object);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.TotalRecords, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void Seed_WhenCsvFileIsValid_SeedsAccountsAndReturnsSuccess()
    {
        var context = CreateDbContext("TestJohnAndJane");       
        var csvPath = CreateTempCsv("AccountId,FirstName,LastName\n1,Jane,Doe\n2,John,Smith");

        var result = AccountSeeder.Seed(context, csvPath, _loggerFactoryMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.TotalRecords, Is.EqualTo(2));
            Assert.That(context.Accounts.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public void Seed_WhenCsvFileIsMalformed_LogsErrorAndReturnsFailure()
    {
        // Arrange
        var context = CreateDbContext("TestMalformedCsv");
        var csvPath = CreateTempCsv("AccountId,FirstName\n1,Jane");
        
        // Act
        var result = AccountSeeder.Seed(context, csvPath, _loggerFactoryMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.Errors[0], Does.Contain("Error occurred while seeding accounts"));
        });
    }
   
    private static MeterReadingsDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<MeterReadingsDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;
        
        return new MeterReadingsDbContext(options);
    }

    private string CreateTempCsv(string content)
    {
        var path = Path.GetTempFileName();
        
        File.WriteAllText(path, content);
        _tempFiles.Add(path);
        
        return path;
    }
}