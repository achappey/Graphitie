using AutoMapper;
using Graphitie.Models;
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
    public Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userId);

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

    public async Task<IEnumerable<Device>> GetDevicesByUser(string userId)
    {
        return await this._microsoftService.GetDevicesByUser(userId);
    }

    public async Task<IEnumerable<User>> GetMembers()
    {
        return await this._microsoftService.GetMembers();
    }


    public async Task<IEnumerable<User>> GetUsers()
    {
        return await this._microsoftService.GetUsers();

    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        return await this._microsoftService.GetEmployees();
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        return await this._microsoftService.GetDevices();
    }

    public async Task<IEnumerable<Graphitie.Models.SecureScore>> GetSecureScores()
    {
        return await this._microsoftService.GetSecureScores();
    }     

    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlerts()
    {
        return await this._microsoftService.GetSecurityAlerts();
    }
    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlertsByUser(string userPrincipalName)
    {
        return await this._microsoftService.GetSecurityAlertsByUser(userPrincipalName);
    }

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        return await this._microsoftService.GetUserRegistrationDetails();
    }

    public async Task<UserRegistrationDetails?> GetUserRegistrationDetailsByUser(string userPrincipalName)
    {
        return await this._microsoftService.GetUserRegistrationDetailsByUser(userPrincipalName);
    }

    public async Task<IEnumerable<SignIn>> GetSignInsByUser(string userId)
    {
        return await this._microsoftService.GetSignInsByUser(userId);
    }

    public async Task<IEnumerable<SignIn>> GetSignIns()
    {
        return await this._microsoftService.GetSignIns();
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