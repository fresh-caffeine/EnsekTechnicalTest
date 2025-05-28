namespace Ensek.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/accounts", () => Results.Ok("List of accounts"))
           .WithName("GetAccounts")
           .WithTags("Accounts");

        app.MapGet("/accounts/{id}", (int id) => Results.Ok($"Account details for {id}"))
           .WithName("GetAccountById")
           .WithTags("Accounts");
    }
}

