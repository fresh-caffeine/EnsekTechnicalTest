namespace Ensek.Api.Endpoints;

public static class MeterReadingEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/meterreadings", () => Results.Ok("List of meter readings"))
            .WithName("GetMeterReadings")
            .WithTags("MeterReadings");

        app.MapGet("/meterreadings/{id}", (int id) => Results.Ok($"Meter reading details for {id}"))
            .WithName("GetMeterReadingById")
            .WithTags("MeterReadings");
    }
}