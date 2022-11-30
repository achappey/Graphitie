using AutoMapper;
using Graphitie.Models;
using Graphitie.Extensions;

namespace Graphitie.Services;

public interface IGraphitieService
{
    public Task<IEnumerable<User>> GetUsers();
    public Task<IEnumerable<User>> GetMembers();
    public Task<IEnumerable<Employee>> GetEmployees();
    public Task<IEnumerable<Group>> GetGroups();    
    public Task AddOwner(string siteId, string userId);
    public Task DeleteOwner(string siteId, string userId);
    public Task SendEmail(string user, string sender, string recipient, string subject, string html);
    public Task<IEnumerable<DevicePerformance>> GetDevicePerformance();
    public Task<int> CopyMemberContacts(string userId, string folderName, string? birthdaySiteId = null);
    public Task AddContacts(string userId, string folderName);
    public Task DeleteMember(string siteId, string userId);
    public Task RenameGroup(string siteId, string name);
    public Task AddTab(string siteId, string name, string url);
}

public class GraphitieService : IGraphitieService
{
    private readonly Microsoft.Graph.GraphServiceClient _graphServiceClient;
    private readonly KeyVaultService _keyVaultService;
    private readonly IMicrosoftService _microsoftService;   
    private readonly IMapper _mapper;
   
    public GraphitieService(Microsoft.Graph.GraphServiceClient graphServiceClient,
    KeyVaultService keyVaultService,
    IMapper mapper,
    MicrosoftService microsoftService)
    {
        _graphServiceClient = graphServiceClient;
        _keyVaultService = keyVaultService;    
        _microsoftService = microsoftService;
        _mapper = mapper;   
    }



    public async Task SendEmail(string user, string sender, string recipient, string subject, string html)
    {
        await this._microsoftService.SendEmail(user, sender, recipient, subject, html);
    }

    public async Task AddTab(string siteId, string name, string url)
    {
        await this._microsoftService.AddTab(siteId, name, url);
    }

    public async Task RenameGroup(string siteId, string name)
    {
        await this._microsoftService.RenameGroup(siteId, name);
    }

    public async Task AddOwner(string siteId, string userId)
    {
        await this._microsoftService.AddGroupOwner(siteId, userId);
    }

    public async Task DeleteMember(string siteId, string userId)
    {
        await this._microsoftService.DeleteGroupMember(siteId, userId);
    }
    public async Task DeleteOwner(string siteId, string userId)
    {
        await this._microsoftService.DeleteGroupOwner(siteId, userId);
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        var items = await this._microsoftService.GetDevices();

        return await WithManagedDevices(items.Select(t => this._mapper.Map<Device>(t)));
    }

    public async Task<IEnumerable<License>> GetLicenses()
    {
        var items = await this._microsoftService.GetLicenses();

        return items.Select(t => this._mapper.Map<License>(t));
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

    public async Task<IEnumerable<Group>> GetGroups()
    {
        var items = await this._microsoftService.GetGroups();

        return items.Select(t => this._mapper.Map<Group>(t));
    }

    public async Task<int> CopyMemberContacts(string userId, string folderName, string? birthdaySiteId = null)
    {

        var graphUser = await _microsoftService.GetUserById(userId);

        if (string.IsNullOrEmpty(graphUser?.Mail))
        {
            return 0;
        }

        Dictionary<string, DateTimeOffset> birthdays = new Dictionary<string, DateTimeOffset>();

        if (!string.IsNullOrEmpty(birthdaySiteId))
        {
            var birthdayEvents = await _microsoftService.GetEvents(birthdaySiteId);

            birthdays = birthdayEvents.ToBirthdays();
        }

        var users = await _microsoftService.GetMembers();

        users = users
        .Where(y => !string.IsNullOrEmpty(y.GivenName))
        .Where(y => !string.IsNullOrEmpty(y.Department))
        .Where(y => !string.IsNullOrEmpty(y.Mail))
         .Select(v => v.WithBirthday(birthdays))
        .Where(y => y.Id != userId);

        var contactFolder = await _microsoftService.EnsureContactFolder(userId, folderName);
        var folderContacts = await _microsoftService.GetContactFolder(userId, contactFolder.Id, GraphConstants.SYNC_REFERENCE);

        var createContacts = users
                .Where(v => folderContacts.All(u => v.Id != u.ToReferenceId()));

        foreach (var userContact in createContacts)
        {
            await _microsoftService.CreateContact(userId, contactFolder.Id, userContact.ToContact(null));
        }

        var updateContacts = users
                .Where(v => folderContacts.Any(u => v.Id == u.ToReferenceId()))
                .Where(v => !v.IsEqual(folderContacts.First(u => v.Id == u.ToReferenceId())))
                .ToDictionary(v => folderContacts.First(u => v.Id == u.ToReferenceId()).Id, v => v);

        foreach (var userContact in updateContacts)
        {
            await _microsoftService.UpdateContact(userId, contactFolder.Id, userContact.Value.ToContact(userContact.Key));
        }

        var deleteContacts = folderContacts
                .Where(v => users.All(u => u.Id != v.ToReferenceId()));

        foreach (var current in deleteContacts)
        {
            await _microsoftService.DeleteContact(userId, contactFolder.Id, current.Id);
        }

        return users.Count();
    }

    public async Task AddContacts(string userId, string folderName)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var items = await this._microsoftService.GetMembers();

        return items.Select(t => this._mapper.Map<Employee>(t));
    }

    public async Task<IEnumerable<DevicePerformance>> GetDevicePerformance()
    {
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

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        var items = await this._microsoftService.GetUserRegistrationDetails();

        return items.Select(t => this._mapper.Map<UserRegistrationDetails>(t));
    }

    public async Task<IEnumerable<SignIn>> GetSignIns()
    {
        var items = await this._microsoftService.GetSignIns();

        return items.Select(t => this._mapper.Map<SignIn>(t));
    }

}