using AutoMapper;
using Graphitie.Models;
using Graphitie.Extensions;
using Graphitie.Connectors.WakaTime;

namespace Graphitie.Services;

public interface IGraphitieService
{
    public Task<IEnumerable<User>> GetUsers();
    public Task<IEnumerable<User>> GetMembers();
    public Task<IEnumerable<Employee>> GetEmployees();
    public Task<IEnumerable<Repository>> GetRepositories();
    public Task<IEnumerable<Language>> GetLanguages();
    public Task<IEnumerable<Device>> GetDevicesByUser(string userId);
    public Task SendEmail(string user, string sender, string recipient, string subject, string html);
    public Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userId);
    public Task<IEnumerable<DevicePerformance>> GetDevicePerformance();

}

public class GraphitieService : IGraphitieService
{
    private readonly Microsoft.Graph.GraphServiceClient _graphServiceClient;
    private readonly KeyVaultService _keyVaultService;
    private readonly IDuolingoService _duolingoService;
    private readonly IMicrosoftService _microsoftService;
    private readonly WakaTime _wakatime;
    private readonly IMapper _mapper;
    private readonly Octokit.GitHubClient _gitHubClient;

    public GraphitieService(Microsoft.Graph.GraphServiceClient graphServiceClient,
    KeyVaultService keyVaultService,
    IMapper mapper,
    DuolingoService duolingoService,
    MicrosoftService microsoftService,
    WakaTime wakatime,
    Octokit.GitHubClient gitHubClient)
    {
        _graphServiceClient = graphServiceClient;
        _keyVaultService = keyVaultService;
        _duolingoService = duolingoService;
        _wakatime = wakatime;
        _microsoftService = microsoftService;
        _mapper = mapper;
        _gitHubClient = gitHubClient;
    }

    public async Task SendEmail(string user, string sender, string recipient, string subject, string html)
    {
        await this._microsoftService.SendEmail(user, sender, recipient, subject, html);
    }

    public async Task<IEnumerable<Device>> GetDevicesByUser(string userId)
    {
        var items = await this._microsoftService.GetDevicesByUser(userId);

        return await WithManagedDevices(items.Select(t => this._mapper.Map<Device>(t)));
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        var items = await this._microsoftService.GetDevices();

        return await WithManagedDevices(items.Select(t => this._mapper.Map<Device>(t)));
    }

    private async Task<IEnumerable<Device>> WithManagedDevices(IEnumerable<Device> devices)
    {
        var managedDevices = await this._microsoftService.GetManagedDevices();

        return devices
            .Select(t => t
                .WithManagedDevice(managedDevices.FirstOrDefault(u => u.AzureADDeviceId == t.DeviceId)));

    }


    public async Task<IEnumerable<User>> GetMembers()
    {
        var items = await this._microsoftService.GetMembers();

        return items.Select(t => this._mapper.Map<User>(t));
    }


    public async Task<IEnumerable<User>> GetUsers()
    {
        var items = await this._microsoftService.GetUsers();

        return items.Select(t => this._mapper.Map<User>(t));
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var items = await this._microsoftService.GetMembers();

        return items.Select(t => this._mapper.Map<Employee>(t));
    }

    public async Task<IEnumerable<DevicePerformance>> GetDevicePerformance()
    {
        //        var items2 = await this._microsoftService.GetDeviceStartupPerformance();
        var items = await this._microsoftService.GetDevicePerformance();
        return items.Select(t => this._mapper.Map<DevicePerformance>(t));
    }


    public async Task<IEnumerable<SecureScore>> GetSecureScores()
    {
        var items = await this._microsoftService.GetSecureScores();

        return items.Select(t => this._mapper.Map<SecureScore>(t));
    }

    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlerts()
    {
        var items = await this._microsoftService.GetSecurityAlerts();

        return items.Select(t => this._mapper.Map<SecurityAlert>(t));
    }
    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlertsByUser(string userPrincipalName)
    {
        var items = await this._microsoftService.GetSecurityAlertsByUser(userPrincipalName);

        return items.Select(t => this._mapper.Map<SecurityAlert>(t));
    }

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        var items = await this._microsoftService.GetUserRegistrationDetails();

        return items.Select(t => this._mapper.Map<UserRegistrationDetails>(t));
    }

    public async Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userPrincipalName)
    {
        var item = await this._microsoftService.GetUserRegistrationDetailsByUser(userPrincipalName);

        return item != null ? this._mapper.Map<UserRegistrationDetails>(item) : null;

    }

    public async Task<IEnumerable<SignIn>> GetSignInsByUser(string userId)
    {
        var items = await this._microsoftService.GetSignInsByUser(userId);

        return items.Select(t => this._mapper.Map<SignIn>(t));
    }

    public async Task<IEnumerable<SignIn>> GetSignIns()
    {
        var items = await this._microsoftService.GetSignIns();

        return items.Select(t => this._mapper.Map<SignIn>(t));
    }

    public async Task<IEnumerable<Repository>> GetRepositories()
    {
        var githubItems = await this.GetGitHubUsers();

        List<Repository> result = new List<Repository>();

        foreach (var github in githubItems)
        {
            var repos = await _gitHubClient.Repository.GetAllForUser(github);

            result.AddRange(
                repos
                .Select(t => this._mapper.Map<Repository>(t)));
        }

        return result;
    }

    public async Task<IEnumerable<Language>> GetLanguages()
    {
        return await this._duolingoService.GetLanguages();
    }


    private async Task<IEnumerable<Activity>> GetWakaTimeActivities()
    {
        var items = await this.GetWakaTimeUsers();

        List<Activity> result = new List<Activity>();

        foreach (var item in items)
        {
            var repos = await _wakatime.GetDurations(item, DateTime.Now);

            result.AddRange(
                repos
                .Select(t => this._mapper.Map<Activity>(t)));
        }

        return result.OrderByDescending(y => y.DateTime);
    }

    private async Task<IEnumerable<Activity>> GetGithubActivities()
    {
        var githubItems = await this.GetGitHubUsers();

        List<Activity> result = new List<Activity>();

        foreach (var github in githubItems)
        {
            var repos = await _gitHubClient.Activity.Events.GetAllUserPerformedPublic(github);

            result.AddRange(
                repos
                .Select(t => this._mapper.Map<Activity>(t)));
        }

        return result.OrderByDescending(y => y.DateTime);
    }

    public async Task<IEnumerable<Activity>> GetActivities()
    {
        var duolingoItems = await this._duolingoService.GetActivities();
        var githubItems = await this.GetWakaTimeActivities();

        return duolingoItems
        .Concat(githubItems)
        .OrderByDescending(u => u.DateTime);
    }


    private async Task<IEnumerable<string>> GetGitHubUsers()
    {
        var items = this._keyVaultService.GetSecretProperties("github");

        List<string> result = new List<string>();

        foreach (var item in items)
        {
            var secret = await this._keyVaultService.GetSecret(item.Name);

            if (secret != null)
            {
                result.Add(secret.Value);
            }
        }

        return result;

    }

    private async Task<IEnumerable<string>> GetWakaTimeUsers()
    {
        var items = this._keyVaultService.GetSecretProperties("wakatime");

        List<string> result = new List<string>();

        foreach (var item in items)
        {
            var secret = await this._keyVaultService.GetSecret(item.Name);

            if (secret != null)
            {
                result.Add(secret.Value);
            }
        }

        return result;

    }
}