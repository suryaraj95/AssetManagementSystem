using System;

namespace AssetManagement.API.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? BranchId { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
    }
}
