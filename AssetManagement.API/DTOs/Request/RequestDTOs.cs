using System;
using System.Collections.Generic;

namespace AssetManagement.API.DTOs.Request
{
    public class CreateRequestDto
    {
        public string RequestType { get; set; } = "New"; // "New" or "Replacement"
        public Guid? AssetTypeId { get; set; }
        public Guid? ReplacementAssetId { get; set; }
        public string? Reason { get; set; }
    }

    public class ApproveRequestDto
    {
        public Guid? AssignedAssetId { get; set; } // Used by Admin
        public string? RejectionReason { get; set; } // Used if rejecting
    }

    public class RequestResponseDto
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public Guid RequestedById { get; set; }
        public string RequestedByName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public Guid? AssetTypeId { get; set; }
        public string? AssetTypeName { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid? AssignedAssetId { get; set; }
        public string? RejectionReason { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
