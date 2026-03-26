using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.API.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var totalAssets = await _context.Assets.CountAsync();
        var assignedAssets = await _context.Assets.CountAsync(a => a.Status == "Assigned");
        var availableAssets = await _context.Assets.CountAsync(a => a.Status == "Available");
        var pendingRequests = await _context.AssetRequests.CountAsync(r => r.Status == "Pending");

        var byCategory = await _context.Assets
            .Include(a => a.AssetType)
            .ThenInclude(t => t!.Category)
            .GroupBy(a => a.AssetType != null && a.AssetType.Category != null ? a.AssetType.Category.Name : "Unknown")
            .Select(g => new CategoryCountDto { CategoryName = g.Key, Count = g.Count() })
            .ToListAsync();

        var byCondition = await _context.Assets
            .GroupBy(a => a.Condition)
            .Select(g => new ConditionCountDto { Condition = g.Key, Count = g.Count() })
            .ToListAsync();

        return new DashboardSummaryDto
        {
            TotalAssets = totalAssets,
            AssignedAssets = assignedAssets,
            AvailableAssets = availableAssets,
            PendingRequests = pendingRequests,
            AssetsByCategory = byCategory,
            AssetsByCondition = byCondition
        };
    }
}
