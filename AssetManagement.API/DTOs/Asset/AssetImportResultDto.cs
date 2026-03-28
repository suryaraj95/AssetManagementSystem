using System.Collections.Generic;

namespace AssetManagement.API.DTOs.Asset
{
    public class AssetImportResultDto
    {
        public bool Success { get; set; }
        public int UploadedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
