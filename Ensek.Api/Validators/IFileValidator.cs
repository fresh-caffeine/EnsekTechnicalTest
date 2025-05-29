namespace Ensek.Api.Validators;

public interface IFileValidator
{
    bool IsValid(IFormFile? file, out List<string> validationErrors);
}