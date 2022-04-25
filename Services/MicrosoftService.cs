using AutoMapper;
using Graphitie.Models;
using Graphitie.Extensions;

namespace Graphitie.Services;

public interface IMicrosoftService
{
    public Task<IEnumerable<User>> GetUsers();
    public Task<IEnumerable<User>> GetMembers();
    public Task<IEnumerable<SecurityAlert>> GetSecurityAlerts();
    public Task<IEnumerable<SecurityAlert>> GetSecurityAlertsByUser(string userPrincipalName);
    public Task<IEnumerable<Device>> GetDevices();
    public Task<IEnumerable<SignIn>> GetSignIns();
    public Task<IEnumerable<SignIn>> GetSignInsByUser(string userId);
    public Task<IEnumerable<Employee>> GetEmployees();
    public Task<IEnumerable<Device>> GetDevicesByUser(string userId);
    public Task<IEnumerable<Graphitie.Models.SecureScore>> GetSecureScores();
    public Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails();
    public Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userPrincipalName);

}

public class MicrosoftService : IMicrosoftService
{
    private readonly Microsoft.Graph.GraphServiceClient _graphServiceClient;
    private readonly IMapper _mapper;

    public MicrosoftService(Microsoft.Graph.GraphServiceClient graphServiceClient,
    IMapper mapper)
    {
        _graphServiceClient = graphServiceClient;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Device>> GetDevicesByUser(string userId)
    {
        var items = await this._graphServiceClient.Devices
       .Request()
       .Top(100)
       .Expand("registeredOwners")
       .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.Device>(items, 100);

        var managedDevices = await GetManagedDevices();

        return allItems
            .Select(t => this._mapper.Map<Graphitie.Models.Device>(t)
                .WithManagedDevice(managedDevices.FirstOrDefault(u => u.AzureADDeviceId == t.DeviceId)));
    }

    public async Task<IEnumerable<User>> GetMembers()
    {
        var user = await this.GetEnabledMembers();

        return user.Select(t => this._mapper.Map<User>(t));
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

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.User>(items, 500, 1000);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        var allItems = await this.GetGraphUsers();

        return allItems.Select(t => this._mapper.Map<User>(t));
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var items = await this.GetEnabledMembers();

        return items.Select(t => this._mapper.Map<Employee>(t));
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        var items = await this._graphServiceClient.Devices
       .Request()
       .Top(100)
       .Expand("registeredOwners")
       .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.Device>(items, 100);

        var managedDevices = await GetManagedDevices();

        return allItems
            .Select(t => this._mapper.Map<Graphitie.Models.Device>(t)
                .WithManagedDevice(managedDevices.FirstOrDefault(u => u.AzureADDeviceId == t.DeviceId)));
    }

    private async Task<IEnumerable<Microsoft.Graph.ManagedDevice>> GetManagedDevices()
    {
        var items = await this._graphServiceClient.DeviceManagement.ManagedDevices
       .Request()
       .Top(300)
       .GetAsync();

        return await _graphServiceClient.PagedRequest<Microsoft.Graph.ManagedDevice>(items, 200);
    }

    public async Task<IEnumerable<Graphitie.Models.SecureScore>> GetSecureScores()
    {
        var iterator = await this._graphServiceClient.Security.SecureScores.Request()
        .Top(100)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.SecureScore>(iterator);

        return allItems.Select(t => this._mapper.Map<Graphitie.Models.SecureScore>(t));
    }

    public async Task<IEnumerable<Microsoft.Graph.UserExperienceAnalyticsBatteryHealthDevicePerformance>> GetModelScores()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsBatteryHealthDevicePerformance.Request()
        .Top(100)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.UserExperienceAnalyticsBatteryHealthDevicePerformance>(iterator);

        return allItems;
    }

    public async Task<IEnumerable<Microsoft.Graph.UserExperienceAnalyticsAppHealthOSVersionPerformance>> GetAppHealthOSVersionPerformance()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsAppHealthOSVersionPerformance.Request()
        .Top(100)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.UserExperienceAnalyticsAppHealthOSVersionPerformance>(iterator);

        return allItems;
    }

    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlerts()
    {
        var items = await this._graphServiceClient.Security.Alerts
       .Request()
       .Top(100)
       .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.Alert>(items, 100);

        return allItems.Select(t => this._mapper.Map<Graphitie.Models.SecurityAlert>(t));
    }

    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlertsByUser(string userPrincipalName)
    {
        var items = await this._graphServiceClient.Security.Alerts
       .Request()
       .Top(100)
       .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.Alert>(items, 100);

        return allItems
        .Where(t => t.UserStates.Any(y => y.UserPrincipalName == userPrincipalName))
        .Select(t => this._mapper.Map<Graphitie.Models.SecurityAlert>(t));
    }

    public async Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userPrincipalName)
    {
        var iterator = await this._graphServiceClient.Reports.AuthenticationMethods.UserRegistrationDetails.Request()
        .Filter(string.Format("userPrincipalName eq '{0}'", userPrincipalName))
        .Top(1)
        .GetAsync();

        return iterator.Select(t => this._mapper.Map<UserRegistrationDetails>(t)).FirstOrDefault();
    }

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        var iterator = await this._graphServiceClient.Reports.AuthenticationMethods.UserRegistrationDetails.Request()
        .Top(999)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.UserRegistrationDetails>(iterator);

        return allItems.Select(t => this._mapper.Map<UserRegistrationDetails>(t));
    }

    public async Task<IEnumerable<SignIn>> GetSignInsByUser(string userId)
    {
        var iterator = await this._graphServiceClient.AuditLogs.SignIns.Request()
        .Filter(string.Format("userId eq '{0}'", userId))
        .Top(999)
        .GetAsync();

        return iterator.Select(t => this._mapper.Map<SignIn>(t));
    }

    public async Task<IEnumerable<SignIn>> GetSignIns()
    {
        var iterator = await this._graphServiceClient.AuditLogs.SignIns.Request()
        .Filter(string.Format("createdDateTime ge {0}", DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ssZ")))
        .Top(999)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<Microsoft.Graph.SignIn>(iterator, 999);

        return allItems.Select(t => this._mapper.Map<SignIn>(t));
    }

  
}