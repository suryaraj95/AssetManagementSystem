using AssetManagement.API.Data;
using AssetManagement.API.DTOs.Request;
using AssetManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetManagement.API.Services
{
    public interface IRequestService
    {
        Task<List<RequestResponseDto>> GetAllAsync(Guid? userId, string? status);
        Task<RequestResponseDto?> GetByIdAsync(Guid id);
        Task<RequestResponseDto> CreateAsync(CreateRequestDto dto, Guid userId);
        Task ApproveByHrAsync(Guid id, Guid hrId);
        Task ApproveByAdminAsync(Guid id, Guid assetId, Guid adminId);
        Task RejectAsync(Guid id, string reason, Guid userId);
    }

    public class RequestService : IRequestService
    {
        private readonly AppDbContext _db;
        private readonly IAssetService _assetService;

        public RequestService(AppDbContext db, IAssetService assetService)
        {
            _db = db;
            _assetService = assetService;
        }

        public async Task<List<RequestResponseDto>> GetAllAsync(Guid? userId, string? status)
        {
            var query = _db.AssetRequests
                .Include(r => r.RequestedBy)
                .Include(r => r.AssetType)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(r => r.RequestedById == userId.Value);
            if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);

            return await query.OrderByDescending(r => r.CreatedAt)
                              .Select(r => MapToDto(r))
                              .ToListAsync();
        }

        public async Task<RequestResponseDto?> GetByIdAsync(Guid id)
        {
            var req = await _db.AssetRequests
                .Include(r => r.RequestedBy)
                .Include(r => r.AssetType)
                .FirstOrDefaultAsync(r => r.Id == id);
            return req == null ? null : MapToDto(req);
        }

        public async Task<RequestResponseDto> CreateAsync(CreateRequestDto dto, Guid userId)
        {
            var today = DateTime.UtcNow;
            var prefix = $"REQ-{today:yyyyMM}";
            
            // Generate sequence number
            int sequence = await _db.AssetRequests.CountAsync(r => r.RequestNumber.StartsWith(prefix)) + 1;
            string requestNumber = $"{prefix}-{sequence:D4}";

            var req = new AssetRequest
            {
                RequestNumber = requestNumber,
                RequestedById = userId,
                RequestType = dto.RequestType,
                AssetTypeId = dto.AssetTypeId,
                ReplacementAssetId = dto.ReplacementAssetId,
                Reason = dto.Reason,
                Status = "Pending"
            };

            _db.AssetRequests.Add(req);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(req.Id) ?? throw new Exception("Error reloading request.");
        }

        public async Task ApproveByHrAsync(Guid id, Guid hrId)
        {
            var req = await _db.AssetRequests.FindAsync(id);
            if (req == null || req.Status != "Pending") throw new Exception("Invalid request state.");
            
            req.Status = "HRApproved";
            req.ApprovedByHrId = hrId;
            req.HrApprovedAt = DateTimeOffset.UtcNow;
            
            await _db.SaveChangesAsync();
        }

        public async Task ApproveByAdminAsync(Guid id, Guid assetId, Guid adminId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            
            var req = await _db.AssetRequests.FindAsync(id);
            if (req == null || (req.Status != "Pending" && req.Status != "HRApproved")) 
                throw new Exception("Invalid request state.");

            // Assign asset via asset service
            await _assetService.AssignAsync(assetId, req.RequestedById, adminId);

            req.Status = "Assigned";
            req.AssignedAssetId = assetId;
            req.ApprovedByAdminId = adminId;
            req.AdminApprovedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task RejectAsync(Guid id, string reason, Guid userId)
        {
            var req = await _db.AssetRequests.FindAsync(id);
            if (req == null || req.Status == "Assigned" || req.Status == "Rejected" || req.Status == "Cancelled") 
                throw new Exception("Cannot reject request in this state.");

            req.Status = "Rejected";
            req.RejectionReason = reason;

            await _db.SaveChangesAsync();
        }

        private static RequestResponseDto MapToDto(AssetRequest r) => new RequestResponseDto
        {
            Id = r.Id,
            RequestNumber = r.RequestNumber,
            RequestedById = r.RequestedById,
            RequestedByName = r.RequestedBy?.FullName ?? "Unknown",
            RequestType = r.RequestType,
            AssetTypeId = r.AssetTypeId,
            AssetTypeName = r.AssetType?.Name,
            Reason = r.Reason,
            Status = r.Status,
            AssignedAssetId = r.AssignedAssetId,
            RejectionReason = r.RejectionReason,
            CreatedAt = r.CreatedAt
        };
    }
}
