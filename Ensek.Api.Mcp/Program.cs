using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Ensek.Api.Mcp;
using Ensek.Api.Models;
using Microsoft.AspNetCore.Mvc;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton<EnsekService>();

await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello from this MCP test: {message}";
    
    [McpServerTool, Description("Echoes the message back to the client in reverse.")]
    public static string Reverse(string message) => $"hello from this MCP: {new string(message.Reverse().ToArray())}";
}

[McpServerToolType]
public static class AccountTool
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    };
    
    [McpServerTool, Description("Get accounts from the Ensek API.")]
    public static async Task<string> GetAccounts(
        [FromServices] EnsekService ensekService)
    {
        var response = await ensekService.GetAccounts();
        return response.Count > 0 ? JsonSerializer.Serialize(response, Options) : "No accounts found.";
    }
    
    [McpServerTool, Description("Get a specific account by ID from the Ensek API with meter readings.")]
    public static async Task<string> GetAccountById(
        [FromServices] EnsekService ensekService, 
        string accountId)
    {
        var response = await ensekService.GetAccountById(accountId);
        return response.Count > 0 ? JsonSerializer.Serialize(response, Options) : "No account found.";
    }
    
}