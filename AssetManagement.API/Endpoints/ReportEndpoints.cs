using Microsoft.AspNetCore.Authorization;
using AssetManagement.API.Services;

namespace AssetManagement.API.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").RequireAuthorization(policy => policy.RequireRole("Admin", "HR"));

        group.MapGet("/stock-list", (string format, IReportService reportService) =>
        {
            byte[] fileContents = format.ToLower() switch
            {
                "excel" => reportService.GenerateExcelReport(),
                "pdf" => reportService.GeneratePdfReport(),
                "docx" => reportService.GenerateDocxReport(),
                _ => Array.Empty<byte>()
            };

            var contentType = format.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            return Results.File(fileContents, contentType, $"StockReport.{format}");
        });

        group.MapGet("/asset-template", (IReportService reportService) =>
        {
            var fileContents = reportService.GenerateBulkUploadTemplate();
            return Results.File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BulkUploadTemplate.xlsx");
        });
    }
}
