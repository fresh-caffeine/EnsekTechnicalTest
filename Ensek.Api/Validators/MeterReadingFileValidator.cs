namespace Ensek.Api.Validators;

public class MeterReadingFileValidator(
    ILogger<MeterReadingFileValidator> logger
    ): IFileValidator
{
    public bool IsValid(IFormFile? file, out List<string> validationErrors)
    {
        var errors = new List<string>();
        
        if (file == null || file.Length == 0)
        {
            logger.LogError("File validation failed: No file uploaded or file is empty.");
            errors.Add("No file uploaded or file is empty.");
            validationErrors = errors;
            return errors.Count == 0;
        }

        if (file.ContentType != "text/csv")
        {
            logger.LogError("File validation failed: Invalid file type. Only CSV files are allowed.");
            errors.Add("Invalid file type. Only CSV files are allowed.");
        }

        validationErrors = errors;
        return errors.Count == 0;
    }
}