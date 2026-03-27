using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Asset;
using AssetManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetManagement.API.Services
{
    public interface IAssetService
    {
        Task<PaginatedList<AssetResponseDto>> GetAllAsync(int page = 1, int size = 10, Guid? branchId = null, Guid? categoryId = null, Guid? assetTypeId = null, Guid? employeeId = null, string? status = null, string? condition = null, string? search = null);
        Task<AssetResponseDto?> GetByIdAsync(Guid id);
        Task<AssetResponseDto> CreateAsync(CreateAssetDto dto, Guid userId);
        Task<AssetResponseDto> UpdateAsync(Guid id, UpdateAssetDto dto, Guid userId);
        Task AssignAsync(Guid assetId, Guid employeeId, Guid adminId);
        Task UnassignAsync(Guid assetId, Guid adminId);
        Task ChangeStatusAsync(Guid assetId, string status, string condition, string? remarks, Guid adminId);
        Task DeleteAsync(Guid id, Guid adminId);
        Task<List<AssetHistoryDto>> GetHistoryAsync(Guid assetId);
    }

    public class AssetService : IAssetService
    {
        private readonly AppDbContext _db;
        private readonly IAssetIdGeneratorService _idGen;

        public AssetService(AppDbContext db, IAssetIdGeneratorService idGen)
        {
            _db = db;
            _idGen = idGen;
        }

        public async Task<PaginatedList<AssetResponseDto>> GetAllAsync(int page = 1, int size = 10, Guid? branchId = null, Guid? categoryId = null, Guid? assetTypeId = null, Guid? employeeId = null, string? status = null, string? condition = null, string? search = null)
        {
            var query = _db.Assets
                .Include(a => a.AssetType).ThenInclude(t => t!.Category)
                .Include(a => a.Branch)
                .Include(a => a.AssignedEmployee)
                .AsQueryable();

            if (branchId.HasValue) query = query.Where(a => a.BranchId == branchId.Value);
            if (categoryId.HasValue) query = query.Where(a => a.AssetType!.CategoryId == categoryId.Value);
            if (assetTypeId.HasValue) query = query.Where(a => a.AssetTypeId == assetTypeId.Value);
            if (employeeId.HasValue) query = query.Where(a => a.AssignedEmployeeId == employeeId.Value);
            if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(condition)) query = query.Where(a => a.Condition == condition);
            if (!string.IsNullOrEmpty(search)) query = query.Where(a => a.AssetId.Contains(search) || a.SerialNumber!.Contains(search));

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.CreatedAt)
                                   .Skip((page - 1) * size)
                                   .Take(size)
                                   .Select(a => MapToDto(a))
                                   .ToListAsync();

            return new PaginatedList<AssetResponseDto> { Items = items, TotalCount = totalCount, PageNumber = page, PageSize = size };
        }

        public async Task<AssetResponseDto?> GetByIdAsync(Guid id)
        {
            var asset = await _db.Assets
                .Include(a => a.AssetType).ThenInclude(t => t!.Category)
                .Include(a => a.Branch)
                .Include(a => a.AssignedEmployee)
                .Include(a => a.SpecValues).ThenInclude(sv => sv.SpecDefinition)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null) return null;
            return MapToDto(asset);
        }

        public async Task<AssetResponseDto> CreateAsync(CreateAssetDto dto, Guid userId)
        {
            var assetIdStr = await _idGen.GenerateAssetIdAsync(dto.BranchId, dto.AssetTypeId);

            var asset = new Asset
            {
                AssetId = assetIdStr,
                SerialNumber = dto.SerialNumber,
                AssetTypeId = dto.AssetTypeId,
                BranchId = dto.BranchId,
                BrandName = dto.BrandName,
                Status = dto.Status,
                Condition = dto.Condition,
                PurchaseDate = dto.PurchaseDate.HasValue ? DateTime.SpecifyKind(dto.PurchaseDate.Value, DateTimeKind.Unspecified) : null,
                PurchaseCost = dto.PurchaseCost,
                WarrantyExpiry = dto.WarrantyExpiry.HasValue ? DateTime.SpecifyKind(dto.WarrantyExpiry.Value, DateTimeKind.Unspecified) : null,
                Notes = dto.Notes
            };

            foreach (var sv in dto.SpecValues)
            {
                asset.SpecValues.Add(new AssetSpecValue { SpecDefinitionId = sv.Key, Value = sv.Value });
            }

            asset.History.Add(new AssetHistory
            {
                Action = "Purchased",
                PerformedById = userId,
                NewStatus = dto.Status,
                NewCondition = dto.Condition,
                Remarks = "Asset initially registered"
            });

            _db.Assets.Add(asset);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(asset.Id) ?? throw new Exception("Failed to load asset after creation.");
        }

        public async Task<AssetResponseDto> UpdateAsync(Guid id, UpdateAssetDto dto, Guid userId)
        {
            var asset = await _db.Assets.Include(a => a.SpecValues).FirstOrDefaultAsync(a => a.Id == id);
            if (asset == null) throw new Exception("Asset not found");

            asset.BrandName = dto.BrandName;
            asset.SerialNumber = dto.SerialNumber;
            asset.PurchaseDate = dto.PurchaseDate.HasValue ? DateTime.SpecifyKind(dto.PurchaseDate.Value, DateTimeKind.Unspecified) : null;
            asset.PurchaseCost = dto.PurchaseCost;
            asset.WarrantyExpiry = dto.WarrantyExpiry.HasValue ? DateTime.SpecifyKind(dto.WarrantyExpiry.Value, DateTimeKind.Unspecified) : null;
            asset.Notes = dto.Notes;
            asset.UpdatedAt = DateTimeOffset.UtcNow;

            foreach (var sv in dto.SpecValues)
            {
                var existing = asset.SpecValues.FirstOrDefault(v => v.SpecDefinitionId == sv.Key);
                if (existing != null) { existing.Value = sv.Value; }
                else { asset.SpecValues.Add(new AssetSpecValue { SpecDefinitionId = sv.Key, Value = sv.Value }); }
            }

            // Use DbSet for inserted entities instead of modifying an unloaded navigation collection
            _db.AssetHistories.Add(new AssetHistory { AssetId = asset.Id, Action = "Updated", PerformedById = userId, Remarks = "Asset details updated" });

            try 
            {
                await _db.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.FirstOrDefault();
                throw new Exception($"Concurrency exception on {entry?.Metadata.Name}: {ex.Message}");
            }
            return await GetByIdAsync(asset.Id) ?? throw new Exception("Error during reload.");
        }

        public async Task AssignAsync(Guid assetId, Guid employeeId, Guid adminId)
        {
            var asset = await _db.Assets.FindAsync(assetId);
            if (asset == null || asset.Status != "Available") throw new Exception("Asset not available");

            var oldStatus = asset.Status;
            asset.Status = "Assigned";
            asset.AssignedEmployeeId = employeeId;
            asset.AssignedAt = DateTimeOffset.UtcNow;

            _db.AssetHistories.Add(new AssetHistory
            {
                AssetId = asset.Id,
                Action = "Assigned",
                PerformedById = adminId,
                ToEmployeeId = employeeId,
                OldStatus = oldStatus,
                NewStatus = "Assigned",
                Remarks = "Assigned by admin"
            });

            await _db.SaveChangesAsync();
        }

        public async Task UnassignAsync(Guid assetId, Guid adminId)
        {
            var asset = await _db.Assets.FindAsync(assetId);
            if (asset == null || asset.Status != "Assigned") throw new Exception("Asset not assigned");

            var oldStatus = asset.Status;
            var fromEmployeeId = asset.AssignedEmployeeId;
            asset.Status = "Available";
            asset.AssignedEmployeeId = null;
            asset.AssignedAt = null;

            _db.AssetHistories.Add(new AssetHistory
            {
                AssetId = asset.Id,
                Action = "Unassigned",
                PerformedById = adminId,
                FromEmployeeId = fromEmployeeId,
                OldStatus = oldStatus,
                NewStatus = "Available",
                Remarks = "Unassigned by admin"
            });

            await _db.SaveChangesAsync();
        }

        public async Task ChangeStatusAsync(Guid assetId, string status, string condition, string? remarks, Guid adminId)
        {
            var asset = await _db.Assets.FindAsync(assetId);
            if (asset == null) throw new Exception("Asset not found");

            var oldStatus = asset.Status;
            var oldCondition = asset.Condition;
            asset.Status = status;
            asset.Condition = condition;
            
            if (status != "Assigned" && asset.AssignedEmployeeId != null)
            {
                asset.AssignedEmployeeId = null;
                asset.AssignedAt = null;
            }

            _db.AssetHistories.Add(new AssetHistory
            {
                AssetId = asset.Id,
                Action = "ConditionChanged",
                PerformedById = adminId,
                OldStatus = oldStatus,
                NewStatus = status,
                OldCondition = oldCondition,
                NewCondition = condition,
                Remarks = remarks ?? "Status/Condition manually changed"
            });

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid adminId)
        {
            var asset = await _db.Assets.FindAsync(id);
            if (asset == null) throw new Exception("Asset not found");
            
            var histories = await _db.AssetHistories.Where(h => h.AssetId == id).ToListAsync();
            _db.AssetHistories.RemoveRange(histories);

            var specValues = await _db.AssetSpecValues.Where(s => s.AssetId == id).ToListAsync();
            _db.AssetSpecValues.RemoveRange(specValues);

            var reqs1 = await _db.AssetRequests.Where(r => r.AssignedAssetId == id).ToListAsync();
            foreach (var r in reqs1) r.AssignedAssetId = null;

            var reqs2 = await _db.AssetRequests.Where(r => r.ReplacementAssetId == id).ToListAsync();
            foreach (var r in reqs2) r.ReplacementAssetId = null;

            _db.Assets.Remove(asset);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AssetHistoryDto>> GetHistoryAsync(Guid assetId)
        {
            return await _db.AssetHistories
                .Include(h => h.PerformedBy)
                .Include(h => h.FromEmployee)
                .Include(h => h.ToEmployee)
                .Where(h => h.AssetId == assetId)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new AssetHistoryDto
                {
                    Id = h.Id,
                    Action = h.Action,
                    PerformedByName = h.PerformedBy!.FullName,
                    FromEmployeeName = h.FromEmployee != null ? h.FromEmployee.FullName : null,
                    ToEmployeeName = h.ToEmployee != null ? h.ToEmployee.FullName : null,
                    OldStatus = h.OldStatus,
                    NewStatus = h.NewStatus,
                    OldCondition = h.OldCondition,
                    NewCondition = h.NewCondition,
                    Remarks = h.Remarks,
                    CreatedAt = h.CreatedAt
                }).ToListAsync();
        }

        private static AssetResponseDto MapToDto(Asset a) => new AssetResponseDto
        {
            Id = a.Id,
            AssetId = a.AssetId,
            SerialNumber = a.SerialNumber,
            AssetTypeId = a.AssetTypeId,
            AssetTypeName = a.AssetType?.Name ?? "Unknown",
            CategoryName = a.AssetType?.Category?.Name ?? "Unknown",
            BranchId = a.BranchId,
            BranchName = a.Branch?.Name ?? "Unknown",
            BrandName = a.BrandName,
            Status = a.Status,
            Condition = a.Condition,
            AssignedEmployeeId = a.AssignedEmployeeId,
            AssignedEmployeeName = a.AssignedEmployee?.FullName,
            AssignedEmployeeEmpId = a.AssignedEmployee?.EmployeeId,
            AssignedAt = a.AssignedAt,
            PurchaseCost = a.PurchaseCost,
            WarrantyExpiry = a.WarrantyExpiry,
            Specs = a.SpecValues.Select(v => new AssetSpecValueDto {
                SpecDefinitionId = v.SpecDefinitionId,
                SpecName = v.SpecDefinition?.SpecName ?? "Unknown",
                Value = v.Value
            }).ToList()
        };
    }
}
