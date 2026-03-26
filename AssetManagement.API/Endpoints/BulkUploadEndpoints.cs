using Microsoft.AspNetCore.Authorization;
using AssetManagement.API.Services;

namespace AssetManagement.API.Endpoints;

public static class BulkUploadEndpoints
{
    public static void MapBulkUploadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets/bulk-upload").RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (IFormFile file, IBulkUploadService bulkUploadService) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("File is empty.");

            using var stream = file.OpenReadStream();
            var result = await bulkUploadService.ProcessExcelUploadAsync(stream);

            return Results.Ok(result);
        }).DisableAntiforgery(); // Minimal APIs form binding
    }
}
