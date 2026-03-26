using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.API.Models
{
    public class AssetRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string RequestNumber { get; set; } = string.Empty;

        public Guid RequestedById { get; set; }
        [ForeignKey("RequestedById")]
        public User? RequestedBy { get; set; }

        [Required, MaxLength(20)]
        public string RequestType { get; set; } = string.Empty;

        public Guid? AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType? AssetType { get; set; }

        public Guid? ReplacementAssetId { get; set; }
        [ForeignKey("ReplacementAssetId")]
        public Asset? ReplacementAsset { get; set; }

        public string? Reason { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public Guid? ApprovedByHrId { get; set; }
        [ForeignKey("ApprovedByHrId")]
        public User? ApprovedByHr { get; set; }

        public DateTimeOffset? HrApprovedAt { get; set; }

        public Guid? ApprovedByAdminId { get; set; }
        [ForeignKey("ApprovedByAdminId")]
        public User? ApprovedByAdmin { get; set; }

        public DateTimeOffset? AdminApprovedAt { get; set; }

        public Guid? AssignedAssetId { get; set; }
        [ForeignKey("AssignedAssetId")]
        public Asset? AssignedAsset { get; set; }

        public string? RejectionReason { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
