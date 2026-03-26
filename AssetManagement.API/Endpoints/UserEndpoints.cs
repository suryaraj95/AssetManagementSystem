using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AssetManagement.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users")
                           .WithTags("Users")
                           .RequireAuthorization(policy => policy.RequireRole("Admin", "HR"));

            group.MapGet("/", async ([FromQuery] string? role, [FromQuery] string? search, AppDbContext db) =>
            {
                var query = db.Users.Include(u => u.Branch).AsQueryable();
                
                if (!string.IsNullOrEmpty(role))
                    query = query.Where(u => u.Role == role);
                
                if (!string.IsNullOrEmpty(search))
                    query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search) || u.EmployeeId.Contains(search));

                var users = await query.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    EmployeeId = u.EmployeeId,
                    Role = u.Role,
                    BranchId = u.BranchId,
                    BranchName = u.Branch != null ? u.Branch.Name : "N/A",
                    Status = u.Status
                }).ToListAsync();

                return Results.Ok(users);
            });

            group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateUserDto dto, AppDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user == null) return Results.NotFound();

                user.FullName = dto.FullName;
                user.Role = dto.Role;
                user.BranchId = dto.BranchId;
                user.Status = dto.Status;
                user.Department = dto.Department;
                user.Phone = dto.Phone;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}

namespace AssetManagement.API.DTOs.Auth
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? BranchId { get; set; }
        public string Status { get; set; } = "Active";
        public string? Department { get; set; }
        public string? Phone { get; set; }
    }
}
