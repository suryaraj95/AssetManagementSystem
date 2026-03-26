using System;

namespace AssetManagement.API.DTOs.Config
{
    public class SpecDto
    {
        public Guid Id { get; set; }
        public Guid AssetTypeId { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public string SpecDataType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}
