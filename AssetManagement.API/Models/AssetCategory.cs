using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.API.Models
{
    public class AssetCategory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<AssetType> AssetTypes { get; set; } = new List<AssetType>();
    }
}
