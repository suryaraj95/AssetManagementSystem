using AssetManagement.API.Models;

namespace AssetManagement.API.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly string _apiKey;
    private readonly string _fromEmail;

    public EmailService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
        _apiKey = _config["Resend:ApiKey"] ?? string.Empty;
        _fromEmail = _config["Resend:FromEmail"] ?? "noreply@yourdomain.com";
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        if (string.IsNullOrEmpty(_apiKey)) return; // Skip if no API key configured

        var payload = new
        {
            from = _fromEmail,
            to = new[] { to },
            subject,
            html = htmlBody
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = JsonContent.Create(payload);

        await _http.SendAsync(request);
    }

    public async Task SendRequestRaisedNotification(string adminEmail, AssetRequest request, User employee)
    {
        await SendAsync(adminEmail, $"New Asset Request #{request.RequestNumber}",
            $"<h2>New {request.RequestType} Asset Request</h2><p>Employee: {employee.FullName} ({employee.EmployeeId})</p><p>Reason: {request.Reason}</p>");
    }

    public async Task SendAssetAssignedNotification(string employeeEmail, Asset asset)
    {
        var assetTypeName = asset.AssetType?.Name ?? "Unknown";
        await SendAsync(employeeEmail, $"Asset Assigned: {asset.AssetId}",
            $"<h2>Asset Assigned to You</h2><p>Asset ID: {asset.AssetId}</p><p>Type: {assetTypeName}</p><p>Serial: {asset.SerialNumber}</p>");
    }
}
