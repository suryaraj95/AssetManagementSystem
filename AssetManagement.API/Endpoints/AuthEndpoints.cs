using AssetManagement.API.DTOs.Auth;
using AssetManagement.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;

namespace AssetManagement.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Authentication");

            group.MapPost("/login", async (LoginDto dto, IAuthService authService) =>
            {
                var result = await authService.LoginAsync(dto);
                if (result == null) return Results.Unauthorized();
                return Results.Ok(result);
            });

            group.MapPost("/register", async (RegisterDto dto, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.RegisterAsync(dto);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapGet("/me", async (HttpContext context, IAuthService authService) =>
            {
                var userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

                var user = await authService.GetMeAsync(userId);
                if (user == null) return Results.NotFound();
                
                return Results.Ok(user);
            }).RequireAuthorization();
        }
    }
}
