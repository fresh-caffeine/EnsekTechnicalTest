using System.Text;
using System.Text.Json;
using Ensek.Api.Data;
using Ensek.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ensek.Api.IntegrationTests;

public class MeterReadingApiTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    
    private IServiceScope _scope;
    private MeterReadingsDbContext _dbContext;
    
    private readonly JsonSerializerOptions? _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();
        
        CleanupTables();
    }

    [TearDown]
    public void TearDown()
    {
        CleanupTables();
        
        _dbContext.Dispose();
        _scope.Dispose();
        _client.Dispose();
        _factory.Dispose();
    }
    
    [Test]
    public async Task UploadCsvFile_ValidAccount_Success()
    {
        const int accountId = 1247;
        SeedAccount(accountId);
        var csv = $"""
                   AccountId,MeterReadingDateTime,MeterReadValue
                   {accountId},22/04/2024 08:24,100
                   """;

        var result = await CreateAndUploadCsvAsync(csv);

        Assert.Multiple(() =>
        {
            Assert.That(result.Response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            
            Assert.That(result.SuccessContent, Is.Not.Null);
            Assert.That(result.SuccessContent!.SuccessRecordCount, Is.EqualTo(1));
            Assert.That(result.SuccessContent!.FailedRecordCount, Is.EqualTo(0));
            Assert.That(result.SuccessContent!.Errors, Has.Count.EqualTo(0));
            
            Assert.That(_dbContext.MeterReadings.Any(m => m.AccountId == accountId && m.ReadingValue == 100), Is.True);
        });
    }
    
    [Test]
    public async Task UploadCsvFile_UnknownAccount_SucceedsWithFailedRows()
    {
        const int accountId = 1247;
        var csv = $"""
                   AccountId,MeterReadingDateTime,MeterReadValue
                   {accountId},22/04/2024 08:24,100
                   """;

        var result = await CreateAndUploadCsvAsync(csv);

        Assert.Multiple(() =>
        {
            Assert.That(result.Response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            
            Assert.That(result.SuccessContent, Is.Not.Null);
            Assert.That(result.SuccessContent!.SuccessRecordCount, Is.EqualTo(0));
            Assert.That(result.SuccessContent!.FailedRecordCount, Is.EqualTo(1));
            Assert.That(result.SuccessContent!.Errors, Has.Count.EqualTo(1));
            
            Assert.That(_dbContext.MeterReadings.Any(), Is.False);
        });
    }
    
    [TestCase(" ")]
    [TestCase("invalid-id")]
    [TestCase("1234567890123456789012345678901234567890")] // Too long
    [TestCase("123.5")] // Decimal value
    public async Task UploadCsvFile_UnparsableAccountId_Fails(string accountId)
    {
        var csv = $"""
                   AccountId,MeterReadingDateTime,MeterReadValue
                   {accountId},22/04/2024 08:24,100
                   """;

        var result = await CreateAndUploadCsvAsync(csv);

        Assert.Multiple(() =>
        {
            Assert.That(result.Response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            
            Assert.That(result.FailureContent, Is.Not.Null);
            Assert.That(result.FailureContent!.Errors, Has.Count.EqualTo(1));
            
            Assert.That(_dbContext.MeterReadings.Any(), Is.False);
        });
    }
    
    [TestCase("AccountId,MeterReadingDateTime")] // Missing MeterReadValue
    [TestCase("AccountId,MeterReadValue")] // Missing MeterReadingDateTime
    [TestCase("MeterReadingDateTime,MeterReadValue")] // Missing AccountId
    [TestCase("AccountId,FirstName,LastName")] // Missing AccountId
    public async Task UploadCsvFile_MissingRequiredHeader_Fails(string headerRow)
    {
        var csv = $"""
                   {headerRow}
                   9999,22/04/2024 08:24,100
                   """;

        var result = await CreateAndUploadCsvAsync(csv);

        Assert.Multiple(() =>
        {
            Assert.That(result.Response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            
            Assert.That(result.FailureContent, Is.Not.Null);
            Assert.That(result.FailureContent!.Errors, Has.Count.EqualTo(1));
            
            Assert.That(_dbContext.MeterReadings.Any(), Is.False);
        });
    }
    
    [TestCase(" ")]
    [TestCase("invalid-date")]
    [TestCase("31/02/2024 08:24")] // Invalid date
    [TestCase("01/01/2024 25:00")] // Invalid time
    [TestCase("01/01/2024 08:60")] // Invalid minute
    [TestCase("01/01/2024 08:24:61")] // Invalid second
    [TestCase("2024-01-01T08:24:00Z")] // ISO 8601 format
    public async Task UploadCsvFile_UnparsableDate_Fails(string date)
    {
        var csv = $"""
                   AccountId,MeterReadingDateTime,MeterReadValue
                   9999 ,{date},100
                   """;

        var result = await CreateAndUploadCsvAsync(csv);

        Assert.Multiple(() =>
        {
            Assert.That(result.Response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            
            Assert.That(result.FailureContent, Is.Not.Null);
            Assert.That(result.FailureContent!.Errors, Has.Count.EqualTo(1));
            
            Assert.That(_dbContext.MeterReadings.Any(), Is.False);
        });
    }
    
    [TestCase("no-extension", "text/plain")]
    [TestCase("im-a.xls", "application/vnd.ms-excel")]
    [TestCase("im-a.xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [TestCase("im-a.txt", "text/plain" )]
    [TestCase("im-a.pdf", "application/pdf")]
    [TestCase("im-a.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [TestCase("im-a.jpeg", "image/jpeg")]
    public async Task UploadCsvFile_InvalidFileExtension_Fails(string filename, string mimeType)
    {
        const string csv = """
                           AccountId,MeterReadingDateTime,MeterReadValue
                           9999,22/04/2024 08:24,100
                           """;

        var result = await CreateAndUploadCsvAsync(csv, filename, mimeType);

        Assert.Multiple(() =>
        {
            Assert.That(result.Response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            
            Assert.That(result.FailureContent, Is.Not.Null);
            Assert.That(result.FailureContent!.Errors, Has.Count.EqualTo(1));
            
            Assert.That(_dbContext.MeterReadings.Any(), Is.False);
        });
    }
    
    private void SeedAccount(int accountId, string firstName = "John", string lastName = "Doe")
    {
        _dbContext.Accounts.Add(new Account { AccountId = accountId, FirstName = firstName, LastName = lastName });
        _dbContext.SaveChanges();
    }
    
    private async Task<UploadCsvResult> CreateAndUploadCsvAsync(string csv, string fileName = "meterreadings.csv", string mimeType = "text/csv")
    {
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csv));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
        var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", fileName);

        var response = await _client.PostAsync("/meter-reading-uploads", form);
        var responseContent = await response.Content.ReadAsStringAsync();

        return new UploadCsvResult
        {
            Response = response,
            SuccessContent = response.IsSuccessStatusCode? JsonSerializer.Deserialize<UploadCsvResult.UploadSuccessResponse>(responseContent, _options) : null,
            FailureContent = !response.IsSuccessStatusCode? JsonSerializer.Deserialize<UploadCsvResult.UploadFailureResponse>(responseContent, _options) : null
        };
    }
    
    private void CleanupTables()
    {
        _dbContext.MeterReadings.RemoveRange(_dbContext.MeterReadings);
        _dbContext.Accounts.RemoveRange(_dbContext.Accounts);
        _dbContext.SaveChanges();
    }
}