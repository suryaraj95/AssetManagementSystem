using AssetManagement.API.DTOs.Asset;
using AssetManagement.API.Services;
using AssetManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Security.Claims;

namespace AssetManagement.API.Endpoints
{
    public static class AssetEndpoints
    {
        public static void MapAssetEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/assets").WithTags("Assets").RequireAuthorization();

            group.MapGet("/", async (
                [FromQuery] int page, [FromQuery] int size, [FromQuery] Guid? branchId, 
                [FromQuery] Guid? categoryId, [FromQuery] Guid? assetTypeId, [FromQuery] Guid? employeeId,
                [FromQuery] string? status, [FromQuery] string? condition, [FromQuery] string? search, 
                HttpContext context, IAssetService assetService) => 
            {
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                
                // Security boundary: Force an Employee to only search inside their own asset envelope natively.
                if (role == "Employee")
                {
                    employeeId = userId;
                }

                page = page < 1 ? 1 : page;
                size = size < 1 ? 10 : size;
                return Results.Ok(await assetService.GetAllAsync(page, size, branchId, categoryId, assetTypeId, employeeId, status, condition, search));
            });

            group.MapGet("/filters", async (AppDbContext db) =>
            {
                var categories = await db.Assets.Select(a => new { id = a.AssetType!.CategoryId, name = a.AssetType.Category!.Name }).Distinct().ToListAsync();
                var types = await db.Assets.Select(a => new { id = a.AssetTypeId, name = a.AssetType!.Name, categoryId = a.AssetType.CategoryId }).Distinct().ToListAsync();
                var branches = await db.Assets.Select(a => new { id = a.BranchId, name = a.Branch!.Name }).Distinct().ToListAsync();
                var assignees = await db.Assets.Where(a => a.AssignedEmployeeId != null).Select(a => new { id = a.AssignedEmployeeId, fullName = a.AssignedEmployee!.FullName, employeeId = a.AssignedEmployee!.EmployeeId }).Distinct().ToListAsync();
                var statuses = await db.Assets.Select(a => a.Status).Distinct().ToListAsync();
                var conditions = await db.Assets.Select(a => a.Condition).Distinct().ToListAsync();

                return Results.Ok(new { categories, types, branches, assignees, statuses, conditions });
            });

            group.MapGet("/{id:guid}", async (Guid id, IAssetService assetService) =>
            {
                var asset = await assetService.GetByIdAsync(id);
                return asset == null ? Results.NotFound() : Results.Ok(asset);
            });

            group.MapPost("/", async (CreateAssetDto dto, HttpContext context, IAssetService assetService) =>
            {
                var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var asset = await assetService.CreateAsync(dto, userId);
                return Results.Created($"/api/assets/{asset.Id}", asset);
            }).RequireAuthorization(p => p.RequireRole("Admin", "HR"));

            group.MapPut("/{id:guid}", async (Guid id, UpdateAssetDto dto, HttpContext context, IAssetService assetService) =>
            {
                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await assetService.UpdateAsync(id, dto, adminId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("Admin", "HR"));

            group.MapPost("/{id:guid}/assign", async (Guid id, [FromBody] AssignDto dto, HttpContext context, IAssetService assetService) =>
            {
                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await assetService.AssignAsync(id, dto.EmployeeId, adminId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("Admin", "HR"));

            group.MapPost("/{id:guid}/unassign", async (Guid id, HttpContext context, IAssetService assetService) =>
            {
                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await assetService.UnassignAsync(id, adminId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("Admin"));

            group.MapPost("/{id:guid}/status", async (Guid id, [FromBody] StatusDto dto, HttpContext context, IAssetService assetService) =>
            {
                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await assetService.ChangeStatusAsync(id, dto.Status, dto.Condition, dto.Remarks, adminId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("Admin", "HR"));

            group.MapDelete("/{id:guid}", async (Guid id, HttpContext context, IAssetService assetService) =>
            {
                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await assetService.DeleteAsync(id, adminId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("Admin"));

            group.MapGet("/{id:guid}/history", async (Guid id, IAssetService assetService) =>
            {
                return Results.Ok(await assetService.GetHistoryAsync(id));
            });

            group.MapGet("/import-template", async (IAssetService assetService) =>
            {
                var fileBytes = await assetService.GenerateImportTemplateAsync();
                return Results.File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AssetsImportTemplate.xlsx");
            }).RequireAuthorization(p => p.RequireRole("Admin", "HR"));

            group.MapGet("/export", async (
                [FromQuery] Guid? branchId, [FromQuery] Guid? categoryId, [FromQuery] Guid? assetTypeId, 
                [FromQuery] Guid? employeeId, [FromQuery] string? status, [FromQuery] string? condition, 
                [FromQuery] string? search, HttpContext context, IAssetService assetService) => 
            {
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                if (role == "Employee") employeeId = userId;

                var fileBytes = await assetService.ExportAssetsAsync(branchId, categoryId, assetTypeId, employeeId, status, condition, search);
                return Results.File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AssetsExport_{DateTime.UtcNow:yyyyMMdd}.xlsx");
            });

            group.MapPost("/import", async (HttpContext context, IAssetService assetService) =>
            {
                if (!context.Request.HasFormContentType || !context.Request.Form.Files.Any())
                    return Results.BadRequest("No file uploaded.");

                var file = context.Request.Form.Files[0];
                if (file.Length == 0) return Results.BadRequest("Empty file.");

                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                using var stream = file.OpenReadStream();
                
                var result = await assetService.ImportAssetsAsync(stream, adminId);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).RequireAuthorization(p => p.RequireRole("Admin", "HR"));
        }
    }

    public class AssignDto { public Guid EmployeeId { get; set; } }
    public class StatusDto { public string Status { get; set; } = string.Empty; public string Condition { get; set; } = string.Empty; public string? Remarks { get; set; } }
}
