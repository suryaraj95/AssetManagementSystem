using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.API.Models
{
    public class AssetSpecValue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        public Guid SpecDefinitionId { get; set; }
        [ForeignKey("SpecDefinitionId")]
        public SpecDefinition? SpecDefinition { get; set; }

        public string? Value { get; set; }
    }
}
