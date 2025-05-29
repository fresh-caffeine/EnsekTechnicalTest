using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Tests.Services;

public class AccountDbServiceTests
{
    private Mock<ILogger<AccountDbService>> _loggerMock;
    private List<string> _tempFiles;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<AccountDbService>>();
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
        var context = CreateDbContext(nameof(Seed_WhenCsvFileDoesNotExist_ReturnsFailureResult));
        var service = new AccountDbService(_loggerMock.Object);
        
        var result = service.SeedAccounts(context, "nonexistent.csv");
      
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.True);
            Assert.That(result.Errors[0], Does.Contain("CSV file not found"));
        });
    }

    [Test]
    public void Seed_WhenAccountsAlreadyExist_ReturnsSuccessResultWithZeroCount()
    {
        var context = CreateDbContext(nameof(Seed_WhenAccountsAlreadyExist_ReturnsSuccessResultWithZeroCount));
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "John", LastName = "Doe" });
        context.SaveChanges();
        
        var service = new AccountDbService(_loggerMock.Object);
        
        var result = service.SeedAccounts(context, Path.GetTempFileName());
        
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.TotalInserted, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void Seed_WhenCsvFileIsValid_SeedsAccountsAndReturnsSuccess()
    {
        var context = CreateDbContext(nameof(Seed_WhenCsvFileIsValid_SeedsAccountsAndReturnsSuccess));       
        var csvPath = CreateTempCsv("AccountId,FirstName,LastName\n1,Jane,Doe\n2,John,Smith");

        var service = new AccountDbService(_loggerMock.Object);
        
        var result = service.SeedAccounts(context, csvPath);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.TotalInserted, Is.EqualTo(2));
            Assert.That(context.Accounts.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public void Seed_WhenCsvFileIsMalformed_LogsErrorAndReturnsFailure()
    {
        // Arrange
        var context = CreateDbContext(nameof(Seed_WhenCsvFileIsMalformed_LogsErrorAndReturnsFailure));
        var csvPath = CreateTempCsv("AccountId,FirstName\n1,Jane");
        
        // Act
        var service = new AccountDbService(_loggerMock.Object);
        
        var result = service.SeedAccounts(context, csvPath);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.True);
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