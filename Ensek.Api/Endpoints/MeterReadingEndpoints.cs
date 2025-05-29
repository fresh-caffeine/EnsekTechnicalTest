using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Data;
using Ensek.Api.Models;
using Ensek.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.Api.Endpoints;

public static class MeterReadingEndpoints
{
    
    public static void MapMeterReadingEndpoints(this WebApplication app)
    {
        app.MapPost("/meter-reading-uploads", async (
                IFormFile file, 
                [FromServices] IMeterReadingService meterReadingService) =>
            {
                var result = await meterReadingService.ProcessMeterReadingFile(file);
                return result;
            })
            .DisableAntiforgery() // TODO: Remove this in production
            .WithName("PostMeterReadings")
            .WithTags("MeterReadings");
    }
}