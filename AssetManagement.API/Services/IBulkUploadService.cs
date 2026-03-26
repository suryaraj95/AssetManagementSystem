namespace AssetManagement.API.Services;

public class BulkUploadResult
{
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

public interface IBulkUploadService
{
    Task<BulkUploadResult> ProcessExcelUploadAsync(Stream fileStream);
}
