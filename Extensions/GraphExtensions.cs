using Microsoft.Graph;

namespace Graphitie.Extensions;

public static class GraphConstants
{
    public const string SYNC_REFERENCE = "achappey.OutlookSync.ref";
    public const string CONTACT_REFERENCE = "ContactId";
}


public static class GraphExtensions
{

    public static bool IsEqual(this User user, Contact contact)
    {
        if (contact == null)
            return false;

        return (
                object.ReferenceEquals(user.DisplayName, contact.DisplayName) ||
                user.DisplayName != null &&
                user.DisplayName.Equals(contact.DisplayName)
            ) &&
            (
                object.ReferenceEquals(user.Surname, contact.Surname) ||
                user.Surname != null &&
                user.Surname.Equals(contact.Surname)
            ) &&
            (
                object.ReferenceEquals(user.GivenName, contact.GivenName) ||
                user.GivenName != null &&
                user.GivenName.Equals(contact.GivenName)
            ) &&
            (
                object.ReferenceEquals(user.JobTitle, contact.JobTitle) ||
                user.JobTitle != null &&
                user.JobTitle.Equals(contact.JobTitle)
            ) &&
            (
                object.ReferenceEquals(user.OfficeLocation, contact.OfficeLocation) ||
                user.OfficeLocation != null &&
                user.OfficeLocation.Equals(contact.OfficeLocation)
            ) &&
             contact.Phones.Any(g => g.Number == user.MobilePhone || user.BusinessPhones.Contains(g.Number))
            &&
            (
               (!user.Birthday.HasValue && !contact.Birthday.HasValue) ||
                (user.Birthday.HasValue && contact.Birthday.HasValue &&
                object.Equals(user.Birthday.Value.Date, contact.Birthday.Value.Date))
            )
              && contact.EmailAddresses.Any(g => g.Address == user.Mail);
    }

    public static async Task<IEnumerable<List>> GetLists(this GraphServiceClient client, string siteId)
    {
        return await client.Sites[siteId]
        .Lists
        .Request()
        .GetAsync();
    }


    public static async Task<IEnumerable<ListItem>> GetEvents(this GraphServiceClient client, string siteId, string listId)
    {
        List<Microsoft.Graph.QueryOption> options = new List<Microsoft.Graph.QueryOption>
        {
                new Microsoft.Graph.QueryOption("expand", "fields(select=EventDate,Category,ParticipantsPicker)")
        };

        return await client.Sites[siteId]
        .Lists[listId]
        .Items
        .Request(options)
        .GetAsync();
    }

    public static Contact ToContact(this User user, string? id)
    {
        var phones = new List<Phone>();

        if (!string.IsNullOrEmpty(user.MobilePhone))
        {
            phones.Add(new Phone()
            {
                Number = user.MobilePhone,
                Type = PhoneType.Mobile
            });
        }

        if (user.BusinessPhones != null && user.BusinessPhones.Count() > 0)
        {
            phones.AddRange(user.BusinessPhones.Select(z => new Phone()
            {
                Number = z,
                Type = PhoneType.Business
            }));
        }

        var email = new List<TypedEmailAddress>();

        if (!string.IsNullOrEmpty(user.Mail))
        {
            email.Add(new TypedEmailAddress()
            {
                Address = user.Mail
            });
        }

        return new Contact()
        {
            DisplayName = user.DisplayName,
            Department = user.Department,
            GivenName = user.GivenName,
            Id = id,
            Birthday = user.Birthday,
            OfficeLocation = user.OfficeLocation,
            Surname = user.Surname,
            Phones = phones,
            JobTitle = user.JobTitle,
            EmailAddresses = email,
            Extensions = new Microsoft.Graph.ContactExtensionsCollectionPage
            {
                new Microsoft.Graph.OpenTypeExtension()
                {
                    ODataType = "microsoft.graph.openTypeExtension",
                    ExtensionName = GraphConstants.SYNC_REFERENCE,
                    AdditionalData = new Dictionary<string, object>()
                    {
                        { GraphConstants.CONTACT_REFERENCE, user.Id }
                    }
                }
            }
        };

    }

    public static string? ToReferenceId(this Contact contact)
    {
        return contact.Extensions.Any(y => y.Id.EndsWith(GraphConstants.SYNC_REFERENCE))
        ? contact.Extensions.First(y => y.Id.EndsWith(GraphConstants.SYNC_REFERENCE)).AdditionalData[GraphConstants.CONTACT_REFERENCE].ToString()
        : null;
    }
    public static async Task<Contact> UpdateContact(this GraphServiceClient client,
          string userId,
          string folderId,
          Contact contact)
    {
        return await client.Users[userId]
        .ContactFolders[folderId]
        .Contacts[contact.Id]
        .Request()
        .UpdateAsync(contact);
    }

    public static async Task DeleteContact(this GraphServiceClient client,
        string userId,
        string folderId,
        string contactId)
    {
        await client.Users[userId]
        .ContactFolders[folderId]
        .Contacts[contactId]
        .Request()
        .DeleteAsync();
    }

    public static async Task<Contact> CreateContact(this GraphServiceClient client,
         string userId,
         string folderId,
         Contact contact)
    {
        return await client.Users[userId]
        .ContactFolders[folderId]
        .Contacts
        .Request()
        .AddAsync(contact);
    }

    public static async Task<IEnumerable<Contact>> GetContactFolder(this GraphServiceClient client,
       string userId,
       string folderId,
       string extensionName)
    {
        List<QueryOption> options = new List<QueryOption>
        {
                new QueryOption("$top", "999"),
                new QueryOption("$expand", string.Format("Extensions($filter=Id eq '{0}')", extensionName)),
        };

        return await client.Users[userId]
        .ContactFolders[folderId]
        .Contacts
        .Request(options)
        .GetAsync();
    }

    public static async Task<ContactFolder> CreateContactFolder(this GraphServiceClient client, string userId, string name)
    {
        var folder = new ContactFolder()
        {
            DisplayName = name
        };

        return await client.Users[userId]
        .ContactFolders
        .Request()
        .AddAsync(folder);
    }

    public static async Task<IEnumerable<ContactFolder>> SearchContactFolders(this GraphServiceClient client, string userId, string title)
    {
        List<QueryOption> options = new List<QueryOption>
            {
                    new QueryOption("$top", "999"),
                    new QueryOption("$filter", string.Format("displayName eq '{0}'", title)),
            };

        return await client.Users[userId]
        .ContactFolders
        .Request(options)
        .GetAsync();
    }

    public static async Task<ContactFolder> EnsureContactFolder(this GraphServiceClient client, string userId, string name)
    {
        var folders = await client.SearchContactFolders(userId, name);
        var contactFolder = folders.FirstOrDefault();

        if (contactFolder == null)
        {
            contactFolder = await client.CreateContactFolder(userId, name);
        }

        return contactFolder;
    }

    public static async Task<IEnumerable<T>> PagedRequest<T>(this GraphServiceClient client, ICollectionPage<T> page, int pauseAfter = 200, int delay = 1500)
    {
        List<T> result = new List<T>();

        int count = 0;

        var pageIterator = PageIterator<T>
            .CreatePageIterator(
                client,
                page,
                (m) =>
                {
                    count++;
                    result.Add(m);

                    return count < pauseAfter;
                }
            );

        await pageIterator.IterateAsync();

        while (pageIterator.State != PagingState.Complete)
        {
            await Task.Delay(delay);
            count = 0;
            await pageIterator.ResumeAsync();
        }

        return result;

    }
}