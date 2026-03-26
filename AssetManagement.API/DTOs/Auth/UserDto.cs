using System;

namespace AssetManagement.API.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new UserDto();
    }
}
