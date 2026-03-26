using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Auth;
using AssetManagement.API.Helpers;
using AssetManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AssetManagement.API.Services
{
    public interface IAuthService
    {
        Task<TokenDto?> LoginAsync(LoginDto loginDto);
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<UserDto?> GetMeAsync(Guid userId);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext db, JwtHelper jwtHelper)
        {
            _db = db;
            _jwtHelper = jwtHelper;
        }

        public async Task<TokenDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _db.Users
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || user.Status != "Active") return null;

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            var token = _jwtHelper.GenerateToken(user);

            return new TokenDto
            {
                Token = token,
                User = MapToUserDto(user)
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new Exception("Email already exists");

            if (await _db.Users.AnyAsync(u => u.EmployeeId == registerDto.EmployeeId))
                throw new Exception("Employee ID already exists");

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                EmployeeId = registerDto.EmployeeId,
                Role = registerDto.Role,
                BranchId = registerDto.BranchId,
                Phone = registerDto.Phone,
                Department = registerDto.Department
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Reload to get branch info if applied
            if (user.BranchId.HasValue)
            {
                await _db.Entry(user).Reference(u => u.Branch).LoadAsync();
            }

            return MapToUserDto(user);
        }

        public async Task<UserDto?> GetMeAsync(Guid userId)
        {
            var user = await _db.Users
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null) return null;
            return MapToUserDto(user);
        }

        private static UserDto MapToUserDto(User user) => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            EmployeeId = user.EmployeeId,
            Role = user.Role,
            BranchId = user.BranchId,
            BranchName = user.Branch?.Name,
            Status = user.Status
        };
    }
}
