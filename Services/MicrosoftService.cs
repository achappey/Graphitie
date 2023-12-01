using Microsoft.Graph;
using Graphitie.Extensions;

namespace Graphitie.Services;

public interface IMicrosoftService
{
    public Task<IEnumerable<User>> GetUsers();
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserById(string id);
    public Task<IEnumerable<User>> GetMembers();
    public Task<IEnumerable<Alert>> GetSecurityAlerts();
    public Task<IEnumerable<Device>> GetDevices();
    public Task<IEnumerable<Group>> GetGroups();
    public Task<Microsoft.Graph.Event> AddCalenderEvent(string user, Microsoft.Graph.Event _event);
    public Task<IEnumerable<SubscribedSku>> GetLicenses();
    public Task<IEnumerable<ManagedDevice>> GetManagedDevices();
    public Task SendMail(string user, Message message);
    public Task<IEnumerable<SignIn>> GetSignIns(int days = 4, int pageSize = 999, int delay = 500);
    public Task SendEmail(string user, string from, string to, string subject, string html);
    public Task<IEnumerable<Message>> SearchEmail(string user, string fromDate, string toDate, string from = null, string subject = null);
    public Task AddGroupOwner(string siteId, string userId);
    public Task<IEnumerable<UserExperienceAnalyticsDevicePerformance>> GetDevicePerformance();
    public Task<IEnumerable<UserExperienceAnalyticsAppHealthApplicationPerformance>> GetDeviceStartupPerformance();
    public Task AddCalendarPermisson(string addPermissionToUser, string userPermission);
    public Task<IEnumerable<SecureScore>> GetSecureScores();
    public Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails();
    public Task<Microsoft.Graph.ContactFolder> EnsureContactFolder(string userId, string name);
    public Task<IEnumerable<Contact>> GetContactFolder(string userId, string name, string reference);
    public Task<Contact> CreateContact(string userId, string folderId, Contact contact);
    public Task<Contact> UpdateContact(string userId, string folderId, Contact contact);
    public Task DeleteContact(string userId, string folderId, string id);
    public Task<IEnumerable<ListItem>> GetEvents(string siteId);
    public Task DeleteGroupOwner(string siteId, string userId);
    public Task DeleteGroupMember(string siteId, string userId);
    public Task<string> GetSharePointUrlOfGroup(string groupId);
    public Task RenameGroup(string siteId, string name);
    public Task AddTab(string siteId, string name, string url);

}

public class MicrosoftService : IMicrosoftService
{
    private readonly Microsoft.Graph.GraphServiceClient _graphServiceClient;

    public MicrosoftService(Microsoft.Graph.GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public async Task AddTab(string siteId, string name, string url)
    {
        try
        {
            // Get all channels for the specified team
            var allChannels = await _graphServiceClient.Teams[siteId].AllChannels
                .Request()
                .GetAsync();

            // Find the General channel
            var generalChannel = allChannels.FirstOrDefault(a => a.DisplayName == "General");

            // Check if the General channel is found
            if (generalChannel == null)
            {
                throw new InvalidOperationException("General channel not found in the specified team.");
            }

            // Create a new TeamsTab instance
            var newTab = new TeamsTab()
            {
                TeamsAppId = "com.microsoft.teamspace.tab.web",
                DisplayName = name,
                Configuration = new TeamsTabConfiguration()
                {
                    WebsiteUrl = url,
                    ContentUrl = url
                }
            };

            // Add the new tab to the General channel
            await _graphServiceClient.Teams[siteId]
                .Channels[generalChannel.Id]
                .Tabs
                .Request()
                .AddAsync(newTab);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to add the tab to the specified team.", ex);
        }
    }


    public async Task RenameGroup(string siteId, string name)
    {
        var group = await this._graphServiceClient.Groups[siteId]
        .Request()
        .GetAsync();

        group.DisplayName = name;

        await _graphServiceClient.Groups[siteId]
            .Request()
            .UpdateAsync(group);

    }


    public async Task<ContactFolder> EnsureContactFolder(string userId, string name)
    {
        return await _graphServiceClient.EnsureContactFolder(userId, name);
    }

    public async Task<IEnumerable<Contact>> GetContactFolder(string userId, string name, string reference)
    {
        return await _graphServiceClient.GetContactFolder(userId, name, reference);
    }

    public async Task<Contact> CreateContact(string userId, string folderId, Contact contact)
    {
        return await _graphServiceClient.CreateContact(userId, folderId, contact);
    }

    public async Task<Contact> UpdateContact(string userId, string folderId, Contact contact)
    {
        return await _graphServiceClient.UpdateContact(userId, folderId, contact);
    }

    public async Task DeleteContact(string userId, string folderId, string id)
    {
        await _graphServiceClient.DeleteContact(userId, folderId, id);
    }

    public async Task<IEnumerable<ListItem>> GetEvents(string siteId)
    {
        var lists = await _graphServiceClient.GetLists(siteId);
        var eventList = lists.FirstOrDefault(t => t.Name == "Events");

        if (eventList != null)
            return await _graphServiceClient.GetEvents(siteId, eventList.Id);

        return new List<ListItem>();
    }

    public async Task<IEnumerable<User>> GetMembers()
    {
        return await GetEnabledMembers();
    }

    private async Task<IEnumerable<User>> GetEnabledMembers()
    {
        var items = await GetGraphUsers("accountEnabled eq true and userType eq 'Member'");

        return items
        .Where(r => !string.IsNullOrEmpty(r.JobTitle?.Trim()))
        .Where(r => !string.IsNullOrEmpty(r.Mail));
    }


    public async Task<Event> AddCalenderEvent(string user, Event _event)
    {
        return await _graphServiceClient.Users[user]
        .Calendar.Events
        .Request()
        .AddAsync(_event);

    }

    private async Task<IEnumerable<User>> GetGraphUsers(string filter = "")
    {
        var items = await _graphServiceClient.Users
        .Request()
        .Top(500)
        .Filter(filter)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(items, 500, 500);
    }

    public async Task<string> GetSharePointUrlOfGroup(string groupId)
    {
        var groupDrive = await _graphServiceClient.Groups[groupId].Sites["root"].Request().GetAsync();
        return new Uri( groupDrive.WebUrl).AbsolutePath;
    }

    public async Task<IEnumerable<Group>> GetGroups()
    {
        var items = await _graphServiceClient.Groups
        .Request()
        .GetAsync();

        return await _graphServiceClient.PagedRequest(items, 500, 500);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var items = await GetGraphUsers(string.Format("mail eq '{0}'", email));

        return items.FirstOrDefault();
    }

    public async Task<User?> GetUserById(string id)
    {
        return await _graphServiceClient.Users[id]
        .Request()
        .GetAsync();
    }


    public async Task<IEnumerable<User>> GetUsers()
    {
        return await this.GetGraphUsers();
    }

    public async Task AddGroupOwner(string siteId, string userId)
    {
        var directoryObject = new DirectoryObject
        {
            Id = userId
        };

        await this._graphServiceClient.Groups[siteId]
      .Owners
      .References
      .Request()
      .AddAsync(directoryObject);
    }

    public async Task DeleteGroupOwner(string siteId, string userId)
    {
        await _graphServiceClient.Groups[siteId].Owners[userId].Reference
            .Request()
            .DeleteAsync();
    }

    public async Task DeleteGroupMember(string siteId, string userId)
    {
        await _graphServiceClient.Groups[siteId].Members[userId].Reference
            .Request()
            .DeleteAsync();
    }

    public async Task SendMail(string user, Message message)
    {
        await this._graphServiceClient.Users[user]
        .SendMail(message)
        .Request()
        .PostAsync();
    }

    public async Task SendEmail(string user, string from, string to, string subject, string html)
    {
        var message = new Message()
        {
            Subject = subject,
            From = new Recipient()
            {
                EmailAddress = new EmailAddress()
                {
                    Address = from
                }
            },

            ToRecipients = new List<Recipient>()
            {
                new Microsoft.Graph.Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = to
                    }
                }
         },
            Body = new ItemBody()
            {
                ContentType = BodyType.Html,
                Content = html
            }
        };

        await this._graphServiceClient.Users[user]
        .SendMail(message)
        .Request()
        .PostAsync();
    }

    public async Task<IEnumerable<Message>> SearchEmail(string user, string fromDate, string toDate, string from = null, string subject = null)
    {
        string filter = $"receivedDateTime ge {fromDate} and receivedDateTime le {toDate}";

        if (!string.IsNullOrWhiteSpace(from))
        {
            filter += $" and from/emailAddress/address eq '{from}'";
        }
        if (!string.IsNullOrWhiteSpace(subject))
        {
            filter += $" and contains(subject, '{subject}')";
        }

        return await this._graphServiceClient.Users[user]
            .MailFolders.Inbox.Messages
            .Request()
            .Filter(filter)
            .OrderBy("receivedDateTime DESC")
            .GetAsync();
    }



    public async Task AddCalendarPermisson(string addPermissionToUser, string userPermission)
    {
        CalendarPermission newPermission = new CalendarPermission
        {
            EmailAddress = new EmailAddress { Address = userPermission },
            IsInsideOrganization = true,
            Role = CalendarRoleType.Write
        };

        await _graphServiceClient.Users[addPermissionToUser].Calendar.CalendarPermissions.Request().AddAsync(newPermission);
    }

    public async Task<IEnumerable<SubscribedSku>> GetLicenses()
    {
        var items = await _graphServiceClient.SubscribedSkus
              .Request()
              .GetAsync();

        return items;
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        var items = await _graphServiceClient.Devices
        .Request()
        .Top(100)
        .Expand("registeredOwners")
        .GetAsync();

        return await _graphServiceClient.PagedRequest(items, 100, 500);
    }

    public async Task<IEnumerable<ManagedDevice>> GetManagedDevices()
    {
        var items = await _graphServiceClient.DeviceManagement.ManagedDevices
       .Request()
       .Top(300)
       .GetAsync();

        return await _graphServiceClient.PagedRequest(items, 200, 500);
    }

    public async Task<IEnumerable<SecureScore>> GetSecureScores()
    {
        var iterator = await _graphServiceClient.Security.SecureScores.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(iterator);
    }

    public async Task<IEnumerable<UserExperienceAnalyticsBatteryHealthDevicePerformance>> GetModelScores()
    {
        var iterator = await this._graphServiceClient.DeviceManagement.UserExperienceAnalyticsBatteryHealthDevicePerformance.Request()
        .Top(100)
        .GetAsync();

        var allItems = await _graphServiceClient.PagedRequest<UserExperienceAnalyticsBatteryHealthDevicePerformance>(iterator);

        return allItems;
    }

    public async Task<IEnumerable<UserExperienceAnalyticsDevicePerformance>> GetDevicePerformance()
    {
        var iterator = await _graphServiceClient.DeviceManagement.UserExperienceAnalyticsDevicePerformance.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(iterator);

    }

    public async Task<IEnumerable<UserExperienceAnalyticsAppHealthApplicationPerformance>> GetDeviceStartupPerformance()
    {
        var iterator = await _graphServiceClient.DeviceManagement.UserExperienceAnalyticsAppHealthApplicationPerformance.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(iterator);

    }


    public async Task<IEnumerable<UserExperienceAnalyticsAppHealthOSVersionPerformance>> GetAppHealthOSVersionPerformance()
    {
        var iterator = await _graphServiceClient.DeviceManagement.UserExperienceAnalyticsAppHealthOSVersionPerformance.Request()
        .Top(100)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(iterator);
    }

    public async Task<IEnumerable<Alert>> GetSecurityAlerts()
    {
        var items = await _graphServiceClient.Security.Alerts
       .Request()
       .Top(100)
       .GetAsync();

        return await _graphServiceClient.PagedRequest(items, 100);
    }

    public async Task<IEnumerable<UserRegistrationDetails>> GetUserRegistrationDetails()
    {
        var iterator = await _graphServiceClient.Reports.AuthenticationMethods.UserRegistrationDetails.Request()
        .Top(999)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(iterator);
    }

    public async Task<IEnumerable<SignIn>> GetSignInsByUser(string userId)
    {

        return await _graphServiceClient.AuditLogs.SignIns.Request()
        .Filter(string.Format("userId eq '{0}'", userId))
        .Top(999)
        .GetAsync();
    }

    public async Task<IEnumerable<SignIn>> GetSignIns(int days = 2, int pageSize = 999, int delay = 500)
    {
        var filter = $"createdDateTime ge {DateTime.UtcNow.AddDays(-days).Date:yyyy-MM-ddTHH:mm:ssZ}";

        var iterator = await _graphServiceClient.AuditLogs.SignIns.Request()
        .Filter(filter)
        .Top(pageSize)
        .GetAsync();

        return await _graphServiceClient.PagedRequest(iterator, pageSize, delay);
    }


}