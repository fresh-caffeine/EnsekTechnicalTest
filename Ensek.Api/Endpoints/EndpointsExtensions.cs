namespace Ensek.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        AccountEndpoints.MapAccountEndpoints(app);
        MeterReadingEndpoints.MapAccountEndpoints(app);
        
        return app;
    }
}