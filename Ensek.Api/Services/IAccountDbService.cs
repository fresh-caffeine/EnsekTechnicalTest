using Ensek.Api.Models;

namespace Ensek.Api.Services;

public interface IAccountDbService
{
    DbInsertResult<string> SeedAccounts(string csvPath);
    Task<List<Account>> GetAccounts();
    Task<Account?> GetAccountById(int accountId);
}