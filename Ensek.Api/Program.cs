using Ensek.Api.Data;
using Ensek.Api.Extensions;
using Ensek.Api.Services;
using Ensek.Api.Validators;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Ensek.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<MeterReadingsDbContext>(
            options => options.UseSqlite(builder.Configuration.GetConnectionString("MeterReadingsDb"))
        );

        // TODO: Uncomment these lines when you want to enable authorization and antiforgery
        // builder.Services.AddAuthorization();
        // builder.Services.AddAntiforgery();


        builder.Services.AddScoped<IAccountDbService, AccountDbService>();
        builder.Services.AddScoped<IRowValidator, MeterReadingRowValidator>();
        builder.Services.AddScoped<IFileValidator, MeterReadingFileValidator>();
        builder.Services.AddScoped<ICsvParser, MeterReadingCsvParser>();
        builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
        builder.Services.AddScoped<IMeterReadingDbService, MeterReadingDbService>();
        
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MigrateDatabase();
        app.SeedAccountsFromCsv("Test_Accounts.csv");
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        // TODO: Uncomment these lines when you want to enable authorization and antiforgery
        // app.UseAuthorization();
        // app.UseAntiforgery();

        app.MapEndpoints();

        app.Run();
    }
}