using System;
using System.Collections.Generic;

namespace AssetManagement.API.DTOs.Config
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<TypeDto> AssetTypes { get; set; } = new();
    }
}
