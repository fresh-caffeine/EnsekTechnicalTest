namespace Ensek.Api.Tests;


public static class TestExtensions
{
    public static object? StatusCode(this IResult result)
    {
        return result.GetType().GetProperty("StatusCode")?.GetValue(result);
    }
}