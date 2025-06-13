using Ensek.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet("/accounts/{accountId:int}", async (
                int accountId, 
                [FromServices] IAccountDbService accountDbService) =>
            {
                var customer = await accountDbService.GetAccountById(accountId);
                return customer is not null ? Results.Ok(customer) : Results.NotFound();
            })
            .WithName("GetAccountById")
            .WithTags("Accounts");
        
        app.MapGet("/accounts", async (
                [FromServices] IAccountDbService accountDbService) =>
            {
                var customers = await accountDbService.GetAccounts();
                
                return Results.Ok(customers);
            })
            .WithName("GetAccounts")
            .WithTags("Accounts");
    }
    
}