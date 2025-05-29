namespace Ensek.Api.Services;

public interface IMeterReadingService
{
    public Task<IResult> ProcessMeterReadingFile(IFormFile? file);
}