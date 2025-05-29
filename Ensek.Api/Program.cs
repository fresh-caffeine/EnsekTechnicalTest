using Ensek.Api.Data;
using Ensek.Api.Extensions;
using Ensek.Api.Services;
using Ensek.Api.Validators;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeterReadingsDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("MeterReadingsDb"))
);

// Add services to the container.
// builder.Services.AddAuthorization();
// builder.Services.AddAntiforgery();


builder.Services.AddScoped<IAccountDbService, AccountDbService>();
builder.Services.AddScoped<IRowValidator, MeterReadingRowValidator>();
builder.Services.AddScoped<IFileValidator, MeterReadingFileValidator>();
builder.Services.AddScoped<ICsvParser, MeterReadingCsvParser>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
builder.Services.AddScoped<IMeterReadingDbService, MeterReadingDbService>();
    
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MigrateDatabase();
app.SeedAccountsFromCsv("Test_Accounts.csv");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// app.UseAuthorization();
// app.UseAntiforgery();

app.MapEndpoints();

app.Run();
