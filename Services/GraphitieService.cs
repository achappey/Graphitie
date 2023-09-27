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
        await _microsoftService.SendEmail(user, sender, recipient, subject, html);
    }

    public async Task SendMail(string user, Mail mail)
    {
        await _microsoftService.SendMail(user, _mapper.Map<Microsoft.Graph.Message>(mail));
    }

    public async Task AddTab(string siteId, string name, string url)
    {
        await _microsoftService.AddTab(siteId, name, url);
    }

    public async Task RenameGroup(string siteId, string name)
    {
        await _microsoftService.RenameGroup(siteId, name);
    }

    public async Task AddOwner(string siteId, string userId)
    {
        await _microsoftService.AddGroupOwner(siteId, userId);
    }

    public async Task DeleteMember(string siteId, string userId)
    {
        await _microsoftService.DeleteGroupMember(siteId, userId);
    }
    public async Task DeleteOwner(string siteId, string userId)
    {
        await _microsoftService.DeleteGroupOwner(siteId, userId);
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        var items = await _microsoftService.GetDevices();

        return await WithManagedDevices(items.Select(_mapper.Map<Device>));
    }

    public async Task<IEnumerable<License>> GetLicenses()
    {
        var items = await _microsoftService.GetLicenses();

        return items.Select(_mapper.Map<License>);
    }

    private async Task<IEnumerable<Device>> WithManagedDevices(IEnumerable<Device> devices)
    {
        var managedDevices = await _microsoftService.GetManagedDevices();

        return devices
            .Select(t => t
                .WithManagedDevice(managedDevices.FirstOrDefault(u => u.AzureADDeviceId == t.DeviceId)));

    }


    public async Task<IEnumerable<User>> GetMembers()
    {
        var items = await _microsoftService.GetMembers();

        return items.Select(_mapper.Map<User>);
    }


    public async Task<IEnumerable<User>> GetUsers()
    {
        var items = await _microsoftService.GetUsers();

        return items.Select(_mapper.Map<User>);
    }

    public async Task<IEnumerable<Group>> GetGroups()
    {
        var items = await _microsoftService.GetGroups();

        return items.Select(_mapper.Map<Group>);
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
            birthdays = (await _microsoftService.GetEvents(birthdaySiteId)).ToBirthdays();
        }

        var users = (await _microsoftService.GetMembers())
            .Where(y => !string.IsNullOrEmpty(y.GivenName) && !string.IsNullOrEmpty(y.Department) && !string.IsNullOrEmpty(y.Mail))
            .Select(v => v.WithBirthday(birthdays))
            .Where(y => y.Id != userId);

        var contactFolder = await _microsoftService.EnsureContactFolder(userId, folderName);
        var folderContacts = await _microsoftService.GetContactFolder(userId, contactFolder.Id, GraphConstants.SYNC_REFERENCE);

        var createContacts = users.Where(v => folderContacts.All(u => v.Id != u.ToReferenceId()));
        var updateContacts = users.Where(v => folderContacts.Any(u => v.Id == u.ToReferenceId() && !v.IsEqual(u)));
        var deleteContacts = folderContacts.Where(v => users.All(u => u.Id != v.ToReferenceId()));

        await Task.WhenAll(createContacts.Select(async v => await _microsoftService.CreateContact(userId, contactFolder.Id, v.ToContact(null))));
        await Task.WhenAll(updateContacts.Select(async v => await _microsoftService.UpdateContact(userId, contactFolder.Id, v.ToContact(folderContacts.First(u => u.Id == v.Id).Id))));
        await Task.WhenAll(deleteContacts.Select(async v => await _microsoftService.DeleteContact(userId, contactFolder.Id, v.Id)));

        return users.Count();
    }


    public async Task AddContacts(string userId, string folderName)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var items = await _microsoftService.GetMembers();

        return items.Select(t => _mapper.Map<Employee>(t));
    }

    public async Task<IEnumerable<DevicePerformance>> GetDevicePerformance()
    {
        var items = await _microsoftService.GetDevicePerformance();
        return items.Select(_mapper.Map<DevicePerformance>);
    }


    public async Task<IEnumerable<SecureScore>> GetSecureScores()
    {
        var items = await _microsoftService.GetSecureScores();

        return items.Select(_mapper.Map<SecureScore>);
    }

    public async Task<IEnumerable<SecurityAlert>> GetSecurityAlerts()
    {
        var items = await _microsoftService.GetSecurityAlerts();

        return items.Select(_mapper.Map<SecurityAlert>);
    }

    public async Task AddCalendarPermisson(string addPermissionToUser, string userPermission)
    {
        await _microsoftService.AddCalendarPermisson(addPermissionToUser, userPermission);
    }

    public async Task<CalendarEvent> AddCalendarEvent(string addPermissionToUser, CalendarEvent _event)
    {
        var item = await _microsoftService.AddCalenderEvent(addPermissionToUser, _mapper.Map<Microsoft.Graph.Event>(_event));

        return _mapper.Map<CalendarEvent>(item);
    }

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        var items = await _microsoftService.GetUserRegistrationDetails();

        return items.Select(_mapper.Map<UserRegistrationDetails>);
    }

    public async Task<IEnumerable<SignIn>> GetSignIns()
    {
        var items = await _microsoftService.GetSignIns();

        return items.Select(_mapper.Map<SignIn>);
    }

}