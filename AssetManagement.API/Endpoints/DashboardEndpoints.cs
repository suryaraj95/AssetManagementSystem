using AssetManagement.API.Services;

namespace AssetManagement.API.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard").RequireAuthorization();

        group.MapGet("/summary", async (IDashboardService dashboardService) =>
        {
            var result = await dashboardService.GetSummaryAsync();
            return Results.Ok(result);
        });
    }
}
