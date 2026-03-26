using AssetManagement.API.DTOs.Request;
using AssetManagement.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;

namespace AssetManagement.API.Endpoints
{
    public static class RequestEndpoints
    {
        public static void MapRequestEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/requests").WithTags("Requests").RequireAuthorization();

            group.MapGet("/", async ([FromQuery] Guid? employeeId, [FromQuery] string? status, HttpContext context, IRequestService reqService) => 
            {
                var role = context.User.FindFirst(ClaimTypes.Role)!.Value;
                var currentUserId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                
                // Employees can only see their own requests
                if (role == "Employee") employeeId = currentUserId;

                return Results.Ok(await reqService.GetAllAsync(employeeId, status));
            });

            group.MapGet("/{id:guid}", async (Guid id, IRequestService reqService) =>
            {
                var req = await reqService.GetByIdAsync(id);
                return req == null ? Results.NotFound() : Results.Ok(req);
            });

            group.MapPost("/", async (CreateRequestDto dto, HttpContext context, IRequestService reqService) =>
            {
                var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var req = await reqService.CreateAsync(dto, userId);
                return Results.Created($"/api/requests/{req.Id}", req);
            }).RequireAuthorization(p => p.RequireRole("Employee"));

            group.MapPut("/{id:guid}/hr-approve", async (Guid id, HttpContext context, IRequestService reqService) =>
            {
                var hrId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await reqService.ApproveByHrAsync(id, hrId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("HR", "Admin")); // Some admins might do HR approvals

            group.MapPut("/{id:guid}/admin-approve", async (Guid id, [FromBody] ApproveRequestDto dto, HttpContext context, IRequestService reqService) =>
            {
                if (!dto.AssignedAssetId.HasValue) return Results.BadRequest("Must provide AssignedAssetId.");
                var adminId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await reqService.ApproveByAdminAsync(id, dto.AssignedAssetId.Value, adminId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("Admin"));

            group.MapPut("/{id:guid}/reject", async (Guid id, [FromBody] ApproveRequestDto dto, HttpContext context, IRequestService reqService) =>
            {
                var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await reqService.RejectAsync(id, dto.RejectionReason ?? "Rejected", userId);
                return Results.NoContent();
            }).RequireAuthorization(p => p.RequireRole("HR", "Admin"));
        }
    }
}
