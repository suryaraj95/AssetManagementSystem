using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.API.Models
{
    public class AssetHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [Required, MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        public Guid? PerformedById { get; set; }
        [ForeignKey("PerformedById")]
        public User? PerformedBy { get; set; }

        public Guid? FromEmployeeId { get; set; }
        [ForeignKey("FromEmployeeId")]
        public User? FromEmployee { get; set; }

        public Guid? ToEmployeeId { get; set; }
        [ForeignKey("ToEmployeeId")]
        public User? ToEmployee { get; set; }

        [MaxLength(20)]
        public string? OldStatus { get; set; }

        [MaxLength(20)]
        public string? NewStatus { get; set; }

        [MaxLength(20)]
        public string? OldCondition { get; set; }

        [MaxLength(20)]
        public string? NewCondition { get; set; }

        public string? Remarks { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
