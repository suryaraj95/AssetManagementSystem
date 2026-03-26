using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Config;
using AssetManagement.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AssetManagement.API.Endpoints
{
    public static class ConfigEndpoints
    {
        public static void MapConfigEndpoints(this IEndpointRouteBuilder app)
        {
            var branches = app.MapGroup("/api/branches").WithTags("Branches").RequireAuthorization();
            
            branches.MapGet("/", async (AppDbContext db) => {
                var list = await db.Branches.Select(b => new BranchDto { Id = b.Id, Name = b.Name, Code = b.Code, Address = b.Address, IsActive = b.IsActive }).ToListAsync();
                return Results.Ok(list);
            });

            branches.MapPost("/", async (BranchDto dto, AppDbContext db) => {
                var branch = new Branch { Name = dto.Name, Code = dto.Code, Address = dto.Address, IsActive = dto.IsActive };
                db.Branches.Add(branch);
                await db.SaveChangesAsync();
                dto.Id = branch.Id;
                return Results.Created($"/api/branches/{branch.Id}", dto);
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            branches.MapPut("/{id:guid}", async (Guid id, BranchDto dto, AppDbContext db) => {
                var branch = await db.Branches.FindAsync(id);
                if (branch == null) return Results.NotFound();
                branch.Name = dto.Name;
                branch.Code = dto.Code;
                branch.Address = dto.Address;
                branch.IsActive = dto.IsActive;
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            branches.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) => {
                var branch = await db.Branches.FindAsync(id);
                if (branch == null) return Results.NotFound();
                db.Branches.Remove(branch);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            var categories = app.MapGroup("/api/categories").WithTags("Categories").RequireAuthorization();

            categories.MapGet("/", async (AppDbContext db) => {
                var list = await db.AssetCategories.Include(c => c.AssetTypes).Select(c => new CategoryDto {
                    Id = c.Id, Name = c.Name, Description = c.Description, IsActive = c.IsActive,
                    AssetTypes = db.AssetTypes.Where(t => t.CategoryId == c.Id).Select(t => new TypeDto { Id = t.Id, CategoryId = t.CategoryId, Name = t.Name, IsActive = t.IsActive }).ToList()
                }).ToListAsync();
                return Results.Ok(list);
            });

            categories.MapPost("/", async (CategoryDto dto, AppDbContext db) => {
                var cat = new AssetCategory { Name = dto.Name, Description = dto.Description, IsActive = dto.IsActive };
                db.AssetCategories.Add(cat);
                await db.SaveChangesAsync();
                dto.Id = cat.Id;
                return Results.Created($"/api/categories/{cat.Id}", dto);
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            categories.MapPut("/{id:guid}", async (Guid id, CategoryDto dto, AppDbContext db) => {
                var cat = await db.AssetCategories.FindAsync(id);
                if (cat == null) return Results.NotFound();
                cat.Name = dto.Name;
                cat.Description = dto.Description;
                cat.IsActive = dto.IsActive;
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            categories.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) => {
                var cat = await db.AssetCategories.FindAsync(id);
                if (cat == null) return Results.NotFound();
                db.AssetCategories.Remove(cat);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            var types = app.MapGroup("/api/asset-types").WithTags("AssetTypes").RequireAuthorization();

            types.MapGet("/", async (Guid? categoryId, AppDbContext db) => {
                var query = db.AssetTypes.Include(t => t.Category).AsQueryable();
                if (categoryId.HasValue) query = query.Where(t => t.CategoryId == categoryId.Value);
                var list = await query.Select(t => new TypeDto { Id = t.Id, CategoryId = t.CategoryId, CategoryName = t.Category!.Name, Name = t.Name, IsActive = t.IsActive }).ToListAsync();
                return Results.Ok(list);
            });

            types.MapPost("/", async (TypeDto dto, AppDbContext db) => {
                var type = new AssetType { CategoryId = dto.CategoryId, Name = dto.Name, IsActive = dto.IsActive };
                db.AssetTypes.Add(type);
                await db.SaveChangesAsync();
                dto.Id = type.Id;
                return Results.Created($"/api/asset-types/{type.Id}", dto);
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            types.MapPut("/{id:guid}", async (Guid id, TypeDto dto, AppDbContext db) => {
                var type = await db.AssetTypes.FindAsync(id);
                if (type == null) return Results.NotFound();
                type.CategoryId = dto.CategoryId;
                type.Name = dto.Name;
                type.IsActive = dto.IsActive;
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            types.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) => {
                var type = await db.AssetTypes.FindAsync(id);
                if (type == null) return Results.NotFound();
                db.AssetTypes.Remove(type);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            var specs = app.MapGroup("/api/specs").WithTags("Specs").RequireAuthorization();

            specs.MapGet("/", async (Guid? assetTypeId, AppDbContext db) => {
                var query = db.SpecDefinitions.AsQueryable();
                if (assetTypeId.HasValue) query = query.Where(s => s.AssetTypeId == assetTypeId.Value);
                var list = await query.OrderBy(s => s.DisplayOrder).Select(s => new SpecDto { 
                    Id = s.Id, AssetTypeId = s.AssetTypeId, SpecName = s.SpecName, SpecDataType = s.SpecDataType, 
                    IsRequired = s.IsRequired, DisplayOrder = s.DisplayOrder 
                }).ToListAsync();
                return Results.Ok(list);
            });

            specs.MapPost("/", async (SpecDto dto, AppDbContext db) => {
                var spec = new SpecDefinition { AssetTypeId = dto.AssetTypeId, SpecName = dto.SpecName, SpecDataType = dto.SpecDataType, IsRequired = dto.IsRequired, DisplayOrder = dto.DisplayOrder };
                db.SpecDefinitions.Add(spec);
                await db.SaveChangesAsync();
                dto.Id = spec.Id;
                return Results.Created($"/api/specs/{spec.Id}", dto);
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            specs.MapPut("/{id:guid}", async (Guid id, SpecDto dto, AppDbContext db) => {
                var spec = await db.SpecDefinitions.FindAsync(id);
                if (spec == null) return Results.NotFound();
                spec.SpecName = dto.SpecName;
                spec.SpecDataType = dto.SpecDataType;
                spec.IsRequired = dto.IsRequired;
                spec.DisplayOrder = dto.DisplayOrder;
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));

            specs.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) => {
                var spec = await db.SpecDefinitions.FindAsync(id);
                if (spec == null) return Results.NotFound();
                db.SpecDefinitions.Remove(spec);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));
        }
    }
}
