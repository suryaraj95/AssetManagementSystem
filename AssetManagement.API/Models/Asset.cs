using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.API.Models
{
    public class Asset
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string AssetId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        public Guid AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType? AssetType { get; set; }

        public Guid BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [MaxLength(100)]
        public string? BrandName { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Available";

        [MaxLength(20)]
        public string Condition { get; set; } = "Good";

        [Column(TypeName = "date")]
        public DateTime? PurchaseDate { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? PurchaseCost { get; set; }

        [Column(TypeName = "date")]
        public DateTime? WarrantyExpiry { get; set; }

        public Guid? AssignedEmployeeId { get; set; }
        [ForeignKey("AssignedEmployeeId")]
        public User? AssignedEmployee { get; set; }

        public DateTimeOffset? AssignedAt { get; set; }

        public DateTimeOffset? DispatchDate { get; set; }

        public string? CourierTrackingAttachment { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<AssetSpecValue> SpecValues { get; set; } = new List<AssetSpecValue>();
        public ICollection<AssetHistory> History { get; set; } = new List<AssetHistory>();
    }
}
