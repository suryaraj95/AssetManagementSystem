namespace AssetManagement.API.Services;

public class ReportService : IReportService
{
    public byte[] GenerateExcelReport()
    {
        // To be implemented with ClosedXML
        return Array.Empty<byte>();
    }

    public byte[] GeneratePdfReport()
    {
        // To be implemented with QuestPDF
        return Array.Empty<byte>();
    }

    public byte[] GenerateDocxReport()
    {
        // To be implemented with DocumentFormat.OpenXml
        return Array.Empty<byte>();
    }

    public byte[] GenerateBulkUploadTemplate()
    {
        // To be implemented
        return Array.Empty<byte>();
    }
}
