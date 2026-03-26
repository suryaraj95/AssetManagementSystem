namespace AssetManagement.API.Services;

public interface IReportService
{
    byte[] GenerateExcelReport();
    byte[] GeneratePdfReport();
    byte[] GenerateDocxReport();
    byte[] GenerateBulkUploadTemplate();
}
