using Ensek.Api.Data;
using Ensek.Api.Endpoints;
using Ensek.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeterReadingsDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("MeterReadingsDb"))
);

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.Services
    .MigrateDatabase()
    .SeedAccountsFromCsv("Test_Accounts.csv");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapEndpoints();

app.Run();
