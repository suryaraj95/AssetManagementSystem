using AssetManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AssetManagement.API.Services;

/// <summary>
/// Executes structured DB queries for each chatbot tool that Gemini can invoke.
/// Returns serialisable objects (JsonElement-ready) that are forwarded back to Gemini.
/// </summary>
public class ChatbotQueryService
{
    private readonly AppDbContext _db;

    public ChatbotQueryService(AppDbContext db)
    {
        _db = db;
    }

    // ─────────────────────────────────────────────────────────────────
    // 1. get_asset_summary
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetSummaryAsync()
    {
        var total      = await _db.Assets.CountAsync();
        var assigned   = await _db.Assets.CountAsync(a => a.Status == "Assigned");
        var available  = await _db.Assets.CountAsync(a => a.Status == "Available");
        var maintenance= await _db.Assets.CountAsync(a => a.Status == "Under Maintenance");
        var retired    = await _db.Assets.CountAsync(a => a.Status == "Retired");
        var pending    = await _db.AssetRequests.CountAsync(r => r.Status == "Pending");

        var byCategory = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .GroupBy(a => a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown")
            .Select(g => new { category = g.Key, count = g.Count() })
            .ToListAsync();

        return new
        {
            total_assets        = total,
            assigned_assets     = assigned,
            available_assets    = available,
            under_maintenance   = maintenance,
            retired_assets      = retired,
            pending_requests    = pending,
            by_category         = byCategory
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // 2. get_assets_by_status
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsByStatusAsync(string status)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.Status.ToLower() == status.ToLower())
            .OrderBy(a => a.AssetId)
            .Select(a => new
            {
                asset_id    = a.AssetId,
                type        = a.AssetType != null ? a.AssetType.Name : "Unknown",
                category    = a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown",
                brand       = a.BrandName,
                branch      = a.Branch != null ? a.Branch.Name : "Unknown",
                condition   = a.Condition,
                assigned_to = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : null
            })
            .ToListAsync();

        return new { status, count = assets.Count, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 3. get_employees_without_assets
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetEmployeesWithoutAssetsAsync()
    {
        var assignedEmployeeIds = await _db.Assets
            .Where(a => a.AssignedEmployeeId != null)
            .Select(a => a.AssignedEmployeeId!.Value)
            .Distinct()
            .ToListAsync();

        var employees = await _db.Users
            .Include(u => u.Branch)
            .Where(u => u.Role == "Employee" && u.Status == "Active" && !assignedEmployeeIds.Contains(u.Id))
            .OrderBy(u => u.FullName)
            .Select(u => new
            {
                employee_id = u.EmployeeId,
                name        = u.FullName,
                email       = u.Email,
                department  = u.Department,
                branch      = u.Branch != null ? u.Branch.Name : "Unknown"
            })
            .ToListAsync();

        return new { count = employees.Count, employees };
    }

    // ─────────────────────────────────────────────────────────────────
    // 4. get_employees_with_assets
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetEmployeesWithAssetsAsync()
    {
        var employees = await _db.Assets
            .Include(a => a.AssignedEmployee).ThenInclude(e => e!.Branch)
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Where(a => a.Status == "Assigned" && a.AssignedEmployee != null)
            .GroupBy(a => new
            {
                Id         = a.AssignedEmployee!.Id,
                Name       = a.AssignedEmployee.FullName,
                EmpId      = a.AssignedEmployee.EmployeeId,
                Department = a.AssignedEmployee.Department,
                Branch     = a.AssignedEmployee.Branch != null ? a.AssignedEmployee.Branch.Name : "Unknown"
            })
            .Select(g => new
            {
                employee_id = g.Key.EmpId,
                name        = g.Key.Name,
                department  = g.Key.Department,
                branch      = g.Key.Branch,
                asset_count = g.Count(),
                assets      = g.Select(a => new { asset_id = a.AssetId, type = a.AssetType != null ? a.AssetType.Name : "Unknown" }).ToList()
            })
            .ToListAsync();

        return new { count = employees.Count, employees };
    }

    // ─────────────────────────────────────────────────────────────────
    // 5. get_assets_by_employee
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsByEmployeeAsync(string employeeNameOrId)
    {
        var search = employeeNameOrId.ToLower();
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.AssignedEmployee != null &&
                        (a.AssignedEmployee.FullName.ToLower().Contains(search) ||
                         a.AssignedEmployee.EmployeeId.ToLower().Contains(search) ||
                         a.AssignedEmployee.Email.ToLower().Contains(search)))
            .Select(a => new
            {
                asset_id    = a.AssetId,
                type        = a.AssetType != null ? a.AssetType.Name : "Unknown",
                category    = a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown",
                brand       = a.BrandName,
                serial      = a.SerialNumber,
                branch      = a.Branch != null ? a.Branch.Name : "Unknown",
                condition   = a.Condition,
                status      = a.Status,
                assigned_to = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : null,
                assigned_at = a.AssignedAt
            })
            .ToListAsync();

        return new { query = employeeNameOrId, count = assets.Count, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 6. get_assets_by_category
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsByCategoryAsync(string categoryName)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.AssetType != null && a.AssetType.Category != null &&
                        a.AssetType.Category.Name.ToLower().Contains(categoryName.ToLower()))
            .OrderBy(a => a.AssetId)
            .Select(a => new
            {
                asset_id    = a.AssetId,
                type        = a.AssetType != null ? a.AssetType.Name : "Unknown",
                brand       = a.BrandName,
                branch      = a.Branch != null ? a.Branch.Name : "Unknown",
                status      = a.Status,
                condition   = a.Condition,
                assigned_to = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : "Unassigned"
            })
            .ToListAsync();

        return new { category = categoryName, count = assets.Count, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 7. get_assets_expiring_warranty
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsExpiringWarrantyAsync(int days = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(days);
        var today  = DateTime.UtcNow;
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.WarrantyExpiry.HasValue &&
                        a.WarrantyExpiry.Value >= today &&
                        a.WarrantyExpiry.Value <= cutoff)
            .OrderBy(a => a.WarrantyExpiry)
            .Select(a => new
            {
                asset_id        = a.AssetId,
                type            = a.AssetType != null ? a.AssetType.Name : "Unknown",
                brand           = a.BrandName,
                branch          = a.Branch != null ? a.Branch.Name : "Unknown",
                warranty_expiry = a.WarrantyExpiry,
                status          = a.Status,
                assigned_to     = a.AssignedEmployee != null ? a.AssignedEmployee.FullName : "Unassigned"
            })
            .ToListAsync();

        return new { days_window = days, count = assets.Count, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 8. get_assets_by_branch
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsByBranchAsync(string branchName)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.Branch != null && a.Branch.Name.ToLower().Contains(branchName.ToLower()))
            .OrderBy(a => a.Status)
            .Select(a => new
            {
                asset_id    = a.AssetId,
                type        = a.AssetType != null ? a.AssetType.Name : "Unknown",
                category    = a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown",
                brand       = a.BrandName,
                status      = a.Status,
                condition   = a.Condition,
                assigned_to = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : "Unassigned"
            })
            .ToListAsync();

        var statusBreakdown = assets
            .GroupBy(a => a.status)
            .Select(g => new { status = g.Key, count = g.Count() })
            .ToList();

        return new { branch = branchName, count = assets.Count, status_breakdown = statusBreakdown, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 9. get_assets_by_condition
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsByConditionAsync(string condition)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.Condition.ToLower() == condition.ToLower())
            .OrderBy(a => a.AssetId)
            .Select(a => new
            {
                asset_id    = a.AssetId,
                type        = a.AssetType != null ? a.AssetType.Name : "Unknown",
                category    = a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown",
                brand       = a.BrandName,
                branch      = a.Branch != null ? a.Branch.Name : "Unknown",
                status      = a.Status,
                assigned_to = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : "Unassigned"
            })
            .ToListAsync();

        return new { condition, count = assets.Count, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 10. get_pending_requests
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetPendingRequestsAsync()
    {
        var requests = await _db.AssetRequests
            .Include(r => r.RequestedBy)
            .Include(r => r.AssetType).ThenInclude(t => t!.Category)
            .Where(r => r.Status == "Pending")
            .OrderBy(r => r.CreatedAt)
            .Select(r => new
            {
                request_number  = r.RequestNumber,
                request_type    = r.RequestType,
                requested_by    = r.RequestedBy != null ? $"{r.RequestedBy.FullName} ({r.RequestedBy.EmployeeId})" : "Unknown",
                asset_type      = r.AssetType != null ? r.AssetType.Name : "Unknown",
                category        = r.AssetType != null && r.AssetType.Category != null ? r.AssetType.Category.Name : "Unknown",
                reason          = r.Reason,
                created_at      = r.CreatedAt
            })
            .ToListAsync();

        return new { count = requests.Count, requests };
    }

    // ─────────────────────────────────────────────────────────────────
    // 11. get_asset_details
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetDetailsAsync(string assetId)
    {
        var asset = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Include(a => a.SpecValues).ThenInclude(sv => sv.SpecDefinition)
            .FirstOrDefaultAsync(a => a.AssetId.ToLower() == assetId.ToLower());

        if (asset == null)
            return new { found = false, message = $"No asset found with ID '{assetId}'." };

        return new
        {
            found           = true,
            asset_id        = asset.AssetId,
            serial_number   = asset.SerialNumber,
            type            = asset.AssetType?.Name,
            category        = asset.AssetType?.Category?.Name,
            brand           = asset.BrandName,
            branch          = asset.Branch?.Name,
            status          = asset.Status,
            condition       = asset.Condition,
            purchase_date   = asset.PurchaseDate,
            purchase_cost   = asset.PurchaseCost,
            warranty_expiry = asset.WarrantyExpiry,
            dispatch_date   = asset.DispatchDate,
            assigned_to     = asset.AssignedEmployee != null ? $"{asset.AssignedEmployee.FullName} ({asset.AssignedEmployee.EmployeeId})" : null,
            assigned_at     = asset.AssignedAt,
            notes           = asset.Notes,
            specs           = asset.SpecValues.Select(sv => new { name = sv.SpecDefinition?.SpecName, value = sv.Value }).ToList()
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // 12. get_recent_asset_history
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetRecentAssetHistoryAsync(int limit = 10)
    {
        if (limit <= 0 || limit > 100) limit = 10;

        var history = await _db.AssetHistories
            .Include(h => h.PerformedBy)
            .Include(h => h.FromEmployee)
            .Include(h => h.ToEmployee)
            .OrderByDescending(h => h.CreatedAt)
            .Take(limit)
            .Select(h => new
            {
                action          = h.Action,
                performed_by    = h.PerformedBy != null ? h.PerformedBy.FullName : "Unknown",
                from_employee   = h.FromEmployee != null ? h.FromEmployee.FullName : null,
                to_employee     = h.ToEmployee   != null ? h.ToEmployee.FullName   : null,
                old_status      = h.OldStatus,
                new_status      = h.NewStatus,
                remarks         = h.Remarks,
                timestamp       = h.CreatedAt
            })
            .ToListAsync();

        return new { limit, count = history.Count, history };
    }

    // ─────────────────────────────────────────────────────────────────
    // 13. get_assets_purchased_in_range
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetAssetsPurchasedInRangeAsync(string fromDate, string toDate)
    {
        if (!DateTime.TryParse(fromDate, out var from) || !DateTime.TryParse(toDate, out var to))
            return new { error = "Invalid date format. Use YYYY-MM-DD." };

        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Where(a => a.PurchaseDate.HasValue &&
                        a.PurchaseDate.Value >= from &&
                        a.PurchaseDate.Value <= to)
            .OrderBy(a => a.PurchaseDate)
            .Select(a => new
            {
                asset_id        = a.AssetId,
                type            = a.AssetType != null ? a.AssetType.Name : "Unknown",
                category        = a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown",
                brand           = a.BrandName,
                branch          = a.Branch != null ? a.Branch.Name : "Unknown",
                status          = a.Status,
                purchase_date   = a.PurchaseDate,
                purchase_cost   = a.PurchaseCost
            })
            .ToListAsync();

        var totalCost = assets.Sum(a => (decimal?)a.purchase_cost) ?? 0;
        return new { from_date = fromDate, to_date = toDate, count = assets.Count, total_cost = totalCost, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // 14. get_high_value_assets
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> GetHighValueAssetsAsync(decimal minCost)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType).ThenInclude(t => t!.Category)
            .Include(a => a.Branch)
            .Include(a => a.AssignedEmployee)
            .Where(a => a.PurchaseCost.HasValue && a.PurchaseCost.Value >= minCost)
            .OrderByDescending(a => a.PurchaseCost)
            .Select(a => new
            {
                asset_id        = a.AssetId,
                type            = a.AssetType != null ? a.AssetType.Name : "Unknown",
                category        = a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown",
                brand           = a.BrandName,
                branch          = a.Branch != null ? a.Branch.Name : "Unknown",
                status          = a.Status,
                purchase_cost   = a.PurchaseCost,
                assigned_to     = a.AssignedEmployee != null ? $"{a.AssignedEmployee.FullName} ({a.AssignedEmployee.EmployeeId})" : "Unassigned"
            })
            .ToListAsync();

        return new { min_cost = minCost, count = assets.Count, assets };
    }

    // ─────────────────────────────────────────────────────────────────
    // Dispatcher: called by ChatbotService after Gemini functionCall
    // ─────────────────────────────────────────────────────────────────
    public async Task<object> DispatchToolCallAsync(string functionName, JsonElement args)
    {
        return functionName switch
        {
            "get_asset_summary"            => await GetAssetSummaryAsync(),
            "get_assets_by_status"         => await GetAssetsByStatusAsync(GetStr(args, "status", "Assigned")),
            "get_employees_without_assets" => await GetEmployeesWithoutAssetsAsync(),
            "get_employees_with_assets"    => await GetEmployeesWithAssetsAsync(),
            "get_assets_by_employee"       => await GetAssetsByEmployeeAsync(GetStr(args, "employee_name_or_id")),
            "get_assets_by_category"       => await GetAssetsByCategoryAsync(GetStr(args, "category_name")),
            "get_assets_expiring_warranty" => await GetAssetsExpiringWarrantyAsync(GetInt(args, "days", 30)),
            "get_assets_by_branch"         => await GetAssetsByBranchAsync(GetStr(args, "branch_name")),
            "get_assets_by_condition"      => await GetAssetsByConditionAsync(GetStr(args, "condition", "Good")),
            "get_pending_requests"         => await GetPendingRequestsAsync(),
            "get_asset_details"            => await GetAssetDetailsAsync(GetStr(args, "asset_id")),
            "get_recent_asset_history"     => await GetRecentAssetHistoryAsync(GetInt(args, "limit", 10)),
            "get_assets_purchased_in_range"=> await GetAssetsPurchasedInRangeAsync(GetStr(args, "from_date"), GetStr(args, "to_date")),
            "get_high_value_assets"        => await GetHighValueAssetsAsync(GetDecimal(args, "min_cost", 0)),
            _                              => new { error = $"Unknown tool: {functionName}" }
        };
    }

    // ─── helpers ───────────────────────────────────────────────────
    private static string GetStr(JsonElement el, string key, string def = "")
    {
        if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String)
            return v.GetString() ?? def;
        return def;
    }

    private static int GetInt(JsonElement el, string key, int def = 0)
    {
        if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty(key, out var v))
        {
            if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i)) return i;
            if (v.ValueKind == JsonValueKind.String && int.TryParse(v.GetString(), out var si)) return si;
        }
        return def;
    }

    private static decimal GetDecimal(JsonElement el, string key, decimal def = 0)
    {
        if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty(key, out var v))
        {
            if (v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out var d)) return d;
            if (v.ValueKind == JsonValueKind.String && decimal.TryParse(v.GetString(), out var sd)) return sd;
        }
        return def;
    }
}
