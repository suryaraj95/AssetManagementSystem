using AssetManagement.API.Models;

namespace AssetManagement.API.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalAssets { get; set; }
    public int AssignedAssets { get; set; }
    public int AvailableAssets { get; set; }
    public int PendingRequests { get; set; }
    public List<CategoryCountDto> AssetsByCategory { get; set; } = new();
    public List<ConditionCountDto> AssetsByCondition { get; set; } = new();
}

public class CategoryCountDto
{
    public string CategoryName { get; set; } = null!;
    public int Count { get; set; }
}

public class ConditionCountDto
{
    public string Condition { get; set; } = null!;
    public int Count { get; set; }
}
