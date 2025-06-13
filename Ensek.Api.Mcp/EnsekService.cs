using System.Net.Http.Json;
using Ensek.Api.Models;

namespace Ensek.Api.Mcp;

public class EnsekService: IDisposable
{
    private readonly HttpClient _httpClient = new();

    private List<Account> _accounts = [];
    
    public async Task<List<Account>> GetAccounts()
    {
        if (_accounts.Count > 0){
            return _accounts;
        }
        
        var url = "http://localhost:5045/accounts";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }
        
        _accounts = await response.Content.ReadFromJsonAsync<List<Account>>() ?? [];
        return _accounts;
    }
    
    
    public async Task<List<Account>> GetAccountById(string? accountId = null)
    {
        if (_accounts.Count == 0)
        {
            await GetAccounts();
        }

        if (string.IsNullOrEmpty(accountId))
        {
            return [];
        }

        var account = _accounts.FirstOrDefault(a => a.AccountId.ToString() == accountId);
        return account != null ? [account] : [];
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}