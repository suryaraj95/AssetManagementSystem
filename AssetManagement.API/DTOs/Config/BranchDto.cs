using System;

namespace AssetManagement.API.DTOs.Config
{
    public class BranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}
