using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.API.Models
{
    public class SpecDefinition
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType? AssetType { get; set; }

        [Required, MaxLength(100)]
        public string SpecName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string SpecDataType { get; set; } = "text";

        public bool IsRequired { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
