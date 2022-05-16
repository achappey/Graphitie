using Microsoft.Graph;
using Graphitie.Extensions;

namespace Graphitie.Services;

public interface IMicrosoftService
{
    public Task<IEnumerable<User>> GetUsers();
    public Task<User?> GetUserByEmail(string email);
    public Task<IEnumerable<User>> GetMembers();
    public Task<IEnumerable<Alert>> GetSecurityAlerts();
    public Task<IEnumerable<Alert>> GetSecurityAlertsByUser(string userPrincipalName);
    public Task<IEnumerable<Device>> GetDevices();
    public Task<IEnumerable<ManagedDevice>> GetManagedDevices();
    public Task<IEnumerable<SignIn>> GetSignIns();
    public Task SendEmail(string from, string to, string subject, string html);
    public Task<IEnumerable<SignIn>> GetSignInsByUser(string userId);
    public Task<IEnumerable<UserExperienceAnalyticsDevicePerformance>> GetDevicePerformance();
    public Task<IEnumerable<UserExperienceAnalyticsAppHealthApplicationPerformance>> GetDeviceStartupPerformance();
    public Task<IEnumerable<Device>> GetDevicesByUser(string userId);
    public Task<IEnumerable<SecureScore>> GetSecureScores();
    public Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails();
    public Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userPrincipalName);

}

public class MicrosoftService : IMicrosoftService
{
    private readonly Microsoft.Graph.GraphServiceClient _graphServiceClient;

    public MicrosoftService(Microsoft.Graph.GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public async Task<byte[]> DownloadFile(string url)
    {
        return new List<byte>().ToArray();
    }

    public async Task<IEnumerable<Device>> GetDevicesByUser(string userId)
    {
        var items = await this.GetDevices();

        return items.Where(t => t.RegisteredOwners.Any(t => t.Id == userId));
    }

    public async Task<IEnumerable<User>> GetMembers()
    {
        return await this.GetEnabledMembers();
    }

    private async Task<IEnumerable<Microsoft.Graph.User>> GetEnabledMembers()
    {
        var items = await this.GetGraphUsers("accountEnabled eq true and userType eq 'Member'");

        return items
        .Where(r => !string.IsNullOrEmpty(r.JobTitle?.Trim()))
        .Where(r => !string.IsNullOrEmpty(r.Mail));
    }

    private async Task<IEnumerable<Microsoft.Graph.User>> GetGraphUsers(string filter = "")
    {
        var items = await _graphServiceClient.Users
        .Request()
        .Top(500)
        .Filter(filter)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.User>(items, 500, 500);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var items = await this.GetGraphUsers(string.Format("mail eq '{0}'", email));

        return items.FirstOrDefault();
    }


    public async Task<IEnumerable<User>> GetUsers()
    {
        return await this.GetGraphUsers();
    }

    public async Task SendEmail(string from, string to, string subject, string html)
    {
        var message = new Message()
        {
            Subject = subject,
            ToRecipients = new List<Microsoft.Graph.Recipient>()
         {
                new Microsoft.Graph.Recipient
                {
                    EmailAddress = new Microsoft.Graph.EmailAddress
                    {
                        Address = to
                    }
                }
         },
            Body = new Microsoft.Graph.ItemBody()
            {
                ContentType = Microsoft.Graph.BodyType.Html,
                Content = html
            }
        };

        await this._graphServiceClient.Users[from]
        .SendMail(message)
        .Request()
        .PostAsync();
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        var items = await this._graphServiceClient.Devices
        .Request()
        .Top(100)
        .Expand("registeredOwners")
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.Device>(items, 100, 500);
    }

    public async Task<IEnumerable<Microsoft.Graph.ManagedDevice>> GetManagedDevices()
    {
        var items = await this._graphServiceClient.DeviceManagement.ManagedDevices
       .Request()
       .Top(300)
       .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.ManagedDevice>(items, 200, 500);
    }

    public async Task<IEnumerable<SecureScore>> GetSecureScores()
    {
        var iterator = await this._graphServiceClient.Security.SecureScores.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.SecureScore>(iterator);
    }

    public async Task<IEnumerable<Microsoft.Graph.UserExperienceAnalyticsBatteryHealthDevicePerformance>> GetModelScores()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsBatteryHealthDevicePerformance.Request()
        .Top(100)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.UserExperienceAnalyticsBatteryHealthDevicePerformance>(iterator);

        return allItems;
    }

    public async Task<IEnumerable<UserExperienceAnalyticsDevicePerformance>> GetDevicePerformance()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsDevicePerformance.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.UserExperienceAnalyticsDevicePerformance>(iterator);

    }

    public async Task<IEnumerable<UserExperienceAnalyticsAppHealthApplicationPerformance>> GetDeviceStartupPerformance()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsAppHealthApplicationPerformance.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.UserExperienceAnalyticsAppHealthApplicationPerformance>(iterator);

    }


    public async Task<IEnumerable<Microsoft.Graph.UserExperienceAnalyticsAppHealthOSVersionPerformance>> GetAppHealthOSVersionPerformance()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsAppHealthOSVersionPerformance.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.UserExperienceAnalyticsAppHealthOSVersionPerformance>(iterator);
    }

    public async Task<IEnumerable<Alert>> GetSecurityAlerts()
    {
        var items = await this._graphServiceClient.Security.Alerts
       .Request()
       .Top(100)
       .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.Alert>(items, 100);
    }

    public async Task<IEnumerable<Alert>> GetSecurityAlertsByUser(string userPrincipalName)
    {
        var items = await this._graphServiceClient.Security.Alerts
       .Request()
       .Top(100)
       .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.Alert>(items, 100);

        return allItems
            .Where(t => t.UserStates.Any(y => y.UserPrincipalName == userPrincipalName));
    }

    public async Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userPrincipalName)
    {
        var iterator = await this._graphServiceClient.Reports.AuthenticationMethods.UserRegistrationDetails.Request()
        .Filter(string.Format("userPrincipalName eq '{0}'", userPrincipalName))
        .Top(1)
        .GetAsync();

        return iterator.FirstOrDefault();
    }

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        var iterator = await this._graphServiceClient.Reports.AuthenticationMethods.UserRegistrationDetails.Request()
        .Top(999)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.UserRegistrationDetails>(iterator);
    }

    public async Task<IEnumerable<Microsoft.Graph.SignIn>> GetSignInsByUser(string userId)
    {
        return await this._graphServiceClient.AuditLogs.SignIns.Request()
        .Filter(string.Format("userId eq '{0}'", userId))
        .Top(999)
        .GetAsync();
    }

    public async Task<IEnumerable<Microsoft.Graph.SignIn>> GetSignIns()
    {
        var iterator = await this._graphServiceClient.AuditLogs.SignIns.Request()
        .Filter(string.Format("createdDateTime ge {0}", DateTime.UtcNow.AddDays(-4).Date.ToString("yyyy-MM-ddTHH:mm:ssZ")))
        .Top(999)
        .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.SignIn>(iterator, 999, 500);
    }


}