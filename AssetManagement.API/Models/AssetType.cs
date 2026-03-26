using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.API.Models
{
    public class AssetType
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public AssetCategory? Category { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
