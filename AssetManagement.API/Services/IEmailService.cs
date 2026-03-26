using AssetManagement.API.Models;

namespace AssetManagement.API.Services;

public interface IEmailService
{
    Task SendRequestRaisedNotification(string adminEmail, AssetRequest request, User employee);
    Task SendAssetAssignedNotification(string employeeEmail, Asset asset);
}
