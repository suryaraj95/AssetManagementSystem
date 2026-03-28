using System;
using System.Collections.Generic;

namespace AssetManagement.API.DTOs.Asset
{
    public class CreateAssetDto
    {
        public string? SerialNumber { get; set; }
        public Guid AssetTypeId { get; set; }
        public Guid BranchId { get; set; }
        public string? BrandName { get; set; }
        public string Status { get; set; } = "Available";
        public string Condition { get; set; } = "Good";
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchaseCost { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public string? Notes { get; set; }
        
        public Dictionary<Guid, string> SpecValues { get; set; } = new();
    }

    public class UpdateAssetDto : CreateAssetDto
    {
    }

    public class AssetResponseDto
    {
        public Guid Id { get; set; }
        public string AssetId { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public Guid AssetTypeId { get; set; }
        public string AssetTypeName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public Guid? AssignedEmployeeId { get; set; }
        public string? AssignedEmployeeName { get; set; }
        public string? AssignedEmployeeEmpId { get; set; }
        public DateTimeOffset? AssignedAt { get; set; }
        public decimal? PurchaseCost { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public DateTimeOffset? DispatchDate { get; set; }
        public bool HasDispatchReceipt { get; set; }
        
        public List<AssetSpecValueDto> Specs { get; set; } = new();
    }

    public class AssetSpecValueDto
    {
        public Guid SpecDefinitionId { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class AssetHistoryDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? PerformedByName { get; set; }
        public string? FromEmployeeName { get; set; }
        public string? ToEmployeeName { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? OldCondition { get; set; }
        public string? NewCondition { get; set; }
        public string? Remarks { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
