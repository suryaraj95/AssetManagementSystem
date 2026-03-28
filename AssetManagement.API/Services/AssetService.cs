using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Asset;
using AssetManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

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
        Task<byte[]> GenerateImportTemplateAsync();
        Task<AssetImportResultDto> ImportAssetsAsync(System.IO.Stream fileStream, Guid adminId);
        Task<byte[]> ExportAssetsAsync(Guid? branchId = null, Guid? categoryId = null, Guid? assetTypeId = null, Guid? employeeId = null, string? status = null, string? condition = null, string? search = null);
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

        public async Task<byte[]> ExportAssetsAsync(Guid? branchId = null, Guid? categoryId = null, Guid? assetTypeId = null, Guid? employeeId = null, string? status = null, string? condition = null, string? search = null)
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

            var assets = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Assets Export");

            var headers = new[] { "Asset ID", "Category", "Type", "Brand & Serial", "Location", "Assignee", "Status", "Condition", "Warranty (dd-mm-yyyy)" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            int row = 2;
            foreach (var a in assets)
            {
                worksheet.Cell(row, 1).Value = a.AssetId;
                worksheet.Cell(row, 2).Value = a.AssetType?.Category?.Name;
                worksheet.Cell(row, 3).Value = a.AssetType?.Name;
                worksheet.Cell(row, 4).Value = $"{a.BrandName} - {a.SerialNumber}";
                worksheet.Cell(row, 5).Value = a.Branch?.Name;
                worksheet.Cell(row, 6).Value = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : "Unassigned";
                worksheet.Cell(row, 7).Value = a.Status;
                worksheet.Cell(row, 8).Value = a.Condition;
                worksheet.Cell(row, 9).Value = a.WarrantyExpiry?.ToString("dd-MM-yyyy") ?? "Not Applicable";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
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

        public async Task<byte[]> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Assets Template");

            var headers = new[] { "Asset ID", "Category", "Type", "Brand", "Serial", "Location", "Condition", "Warranty (dd-mm-yyyy)" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            var lookupSheet = workbook.Worksheets.Add("LookupData");
            lookupSheet.Hide();

            var categories = await _db.AssetCategories.Select(c => c.Name).ToListAsync();
            if (categories.Count > 0)
            {
                for (int i = 0; i < categories.Count; i++) lookupSheet.Cell(i + 1, 1).Value = categories[i];
                workbook.NamedRanges.Add("CategoriesRange", lookupSheet.Range(1, 1, categories.Count, 1));
                worksheet.Range("B2:B5000").CreateDataValidation().List("=CategoriesRange");
            }

            var types = await _db.AssetTypes.Select(t => t.Name).Distinct().ToListAsync();
            if (types.Count > 0)
            {
                for (int i = 0; i < types.Count; i++) lookupSheet.Cell(i + 1, 2).Value = types[i];
                workbook.NamedRanges.Add("TypesRange", lookupSheet.Range(1, 2, types.Count, 2));
                worksheet.Range("C2:C5000").CreateDataValidation().List("=TypesRange");
            }

            var branches = await _db.Branches.Select(b => b.Name).ToListAsync();
            if (branches.Count > 0)
            {
                for (int i = 0; i < branches.Count; i++) lookupSheet.Cell(i + 1, 3).Value = branches[i];
                workbook.NamedRanges.Add("BranchesRange", lookupSheet.Range(1, 3, branches.Count, 3));
                worksheet.Range("F2:F5000").CreateDataValidation().List("=BranchesRange");
            }

            var conditions = new[] { "Good", "Fair", "Poor", "Broken" };
            for (int i = 0; i < conditions.Length; i++) lookupSheet.Cell(i + 1, 4).Value = conditions[i];
            workbook.NamedRanges.Add("ConditionsRange", lookupSheet.Range(1, 4, conditions.Length, 4));
            worksheet.Range("G2:G5000").CreateDataValidation().List("=ConditionsRange");

            var dateValidation = worksheet.Range("H2:H5000").CreateDataValidation();
            dateValidation.Custom("ISNUMBER(H2)");
            dateValidation.ErrorMessage = "Please enter a valid date in dd-mm-yyyy format.";

            worksheet.Columns().AdjustToContents();

            using var stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<AssetImportResultDto> ImportAssetsAsync(System.IO.Stream fileStream, Guid adminId)
        {
            var result = new AssetImportResultDto();
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == "Assets Template") ?? workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1); 

            var assetsToCreate = new List<Asset>();
            int rowNum = 1;

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var types = await _db.AssetTypes.Include(t => t.Category).ToListAsync();
                var branches = await _db.Branches.ToListAsync();
                var existingAssetIds = await _db.Assets.Select(a => a.AssetId).ToListAsync();
                
                var currentSheetAssetIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var row in rows)
                {
                    rowNum++; 

                    var assetIdCol = row.Cell(1).GetString().Trim();
                    var categoryName = row.Cell(2).GetString().Trim();
                    var typeName = row.Cell(3).GetString().Trim();
                    var brandName = row.Cell(4).GetString().Trim();
                    var serialNumber = row.Cell(5).GetString().Trim();
                    var locationName = row.Cell(6).GetString().Trim();
                    var condition = row.Cell(7).GetString().Trim();
                    var warrantyStr = row.Cell(8).GetString().Trim();

                    if (string.IsNullOrEmpty(assetIdCol) && string.IsNullOrEmpty(categoryName) && string.IsNullOrEmpty(typeName))
                        continue; 

                    if (string.IsNullOrEmpty(assetIdCol)) result.Errors.Add($"Row {rowNum}: Asset ID is required.");
                    else
                    {
                        if (existingAssetIds.Any(a => a.Equals(assetIdCol, StringComparison.OrdinalIgnoreCase)))
                            result.Errors.Add($"Row {rowNum}: Asset ID '{assetIdCol}' already exists in the system.");
                        else if (currentSheetAssetIds.Contains(assetIdCol))
                            result.Errors.Add($"Row {rowNum}: Duplicate Asset ID '{assetIdCol}' within the import sheet.");
                        else
                            currentSheetAssetIds.Add(assetIdCol);
                    }

                    if (string.IsNullOrEmpty(categoryName)) result.Errors.Add($"Row {rowNum}: Category is required.");
                    if (string.IsNullOrEmpty(typeName)) result.Errors.Add($"Row {rowNum}: Type is required.");
                    if (string.IsNullOrEmpty(brandName)) result.Errors.Add($"Row {rowNum}: Brand is required.");
                    if (string.IsNullOrEmpty(serialNumber)) result.Errors.Add($"Row {rowNum}: Serial Number is required.");
                    if (string.IsNullOrEmpty(locationName)) result.Errors.Add($"Row {rowNum}: Location is required.");
                    if (string.IsNullOrEmpty(condition)) result.Errors.Add($"Row {rowNum}: Condition is required.");

                    var validConditions = new[] { "Good", "Fair", "Poor", "Broken" };
                    if (!string.IsNullOrEmpty(condition) && !validConditions.Contains(condition, StringComparer.OrdinalIgnoreCase)) 
                        result.Errors.Add($"Row {rowNum}: Invalid Condition '{condition}'.");

                    var branch = branches.FirstOrDefault(b => b.Name.Equals(locationName, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(locationName) && branch == null) 
                        result.Errors.Add($"Row {rowNum}: Location '{locationName}' not found.");

                    var assetType = types.FirstOrDefault(t => 
                        t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) && 
                        t.Category!.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(categoryName) && assetType == null) 
                        result.Errors.Add($"Row {rowNum}: Asset Type '{typeName}' under Category '{categoryName}' not found.");

                    DateTime? warrantyExpiry = null;
                    if (!string.IsNullOrEmpty(warrantyStr) && !warrantyStr.Equals("Not Applicable", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.TryParseExact(warrantyStr, new[] { "dd-MM-yyyy", "dd-M-yyyy", "d-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy", "d/M/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedExact))
                        {
                            warrantyExpiry = DateTime.SpecifyKind(parsedExact, DateTimeKind.Unspecified);
                        }
                        else if (DateTime.TryParse(warrantyStr, out var d))
                        {
                            warrantyExpiry = DateTime.SpecifyKind(d, DateTimeKind.Unspecified);
                        }
                        else if (double.TryParse(warrantyStr, out double excelDate)) 
                        {
                            try {
                                warrantyExpiry = DateTime.SpecifyKind(DateTime.FromOADate(excelDate), DateTimeKind.Unspecified);
                            } catch {
                                result.Errors.Add($"Row {rowNum}: Invalid Warranty Date format '{warrantyStr}'. Use dd-mm-yyyy.");
                            }
                        }
                        else
                        {
                            result.Errors.Add($"Row {rowNum}: Invalid Warranty Date format '{warrantyStr}'. Use dd-mm-yyyy.");
                        }
                    }

                    if (result.Errors.Any()) continue; 

                    var asset = new Asset
                    {
                        AssetId = assetIdCol,
                        AssetTypeId = assetType!.Id,
                        BranchId = branch!.Id,
                        BrandName = brandName,
                        SerialNumber = serialNumber,
                        Status = "Available",
                        Condition = char.ToUpper(condition[0]) + condition.Substring(1).ToLower(),
                        WarrantyExpiry = warrantyExpiry,
                        Notes = "Imported from Excel"
                    };

                    assetsToCreate.Add(asset);
                }

                if (result.Errors.Any())
                {
                    result.Success = false;
                    await transaction.RollbackAsync();
                    return result;
                }

                foreach (var asset in assetsToCreate)
                {
                    _db.Assets.Add(asset);
                }

                await _db.SaveChangesAsync();

                var histories = assetsToCreate.Select(a => new AssetHistory
                {
                    AssetId = a.Id,
                    Action = "Imported",
                    PerformedById = adminId,
                    NewStatus = a.Status,
                    NewCondition = a.Condition,
                    Remarks = "Imported via Excel template."
                }).ToList();

                _db.AssetHistories.AddRange(histories);
                await _db.SaveChangesAsync();
                
                await transaction.CommitAsync();

                result.Success = true;
                result.UploadedCount = assetsToCreate.Count;
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"System Error: {ex.Message}");
                await transaction.RollbackAsync();
                result.Success = false;
                return result;
            }
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
