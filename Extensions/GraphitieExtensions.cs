using Graphitie.Models;
using System.Text.Json;

namespace Graphitie.Extensions;

 public class LookupValue
    {
        public string? Email { get; set; }

    }

public static class GraphitieExtensions
{
     public static Microsoft.Graph.User WithBirthday(this Microsoft.Graph.User user, Dictionary<string, DateTimeOffset> birthdays)
    {
        user.Birthday = !string.IsNullOrEmpty(user.Mail)
                && birthdays.ContainsKey(user.Mail) ? (DateTimeOffset?)birthdays[user.Mail] : null;
        
        return  user;
    }
    
   public static Dictionary<string, DateTimeOffset> ToBirthdays(this IEnumerable<Microsoft.Graph.ListItem> items)
        {
            return items
            .Where(y => y.Fields.AdditionalData.ContainsKey("Category"))
            .Where(y => y.Fields.AdditionalData.ContainsKey("EventDate"))
            .Where(y => y.Fields.AdditionalData.ContainsKey("ParticipantsPicker"))
          .Where(y => y.Fields.AdditionalData["Category"].ToString() == "Verjaardag")
          .SelectMany(y => JsonSerializer.Deserialize<IEnumerable<LookupValue>>(y.Fields.AdditionalData["ParticipantsPicker"].ToString()!)!
          .Select(a =>
          {
              return new
              {
                  Date = DateTimeOffset.Parse(y.Fields.AdditionalData["EventDate"].ToString()!),
                  User = a.Email
              };
          }))
            .GroupBy(y => y.User)
            .ToDictionary(y => y.Key!, y => y.OrderByDescending(a => a.Date).First().Date.AddYears(-18));
        }
        
    public static Device WithManagedDevice(this Device device, Microsoft.Graph.ManagedDevice? managedDevice)
    {
        device.IsEncrypted = managedDevice?.IsEncrypted;
        device.FreeStorageSpaceInBytes = managedDevice?.FreeStorageSpaceInBytes;
        device.TotalStorageSpaceInBytes = managedDevice?.TotalStorageSpaceInBytes;
        device.LastSyncDateTime = managedDevice?.LastSyncDateTime;
        device.Imei = managedDevice?.Imei;
        device.ManagedDeviceId = managedDevice?.Id;
        device.SerialNumber = managedDevice?.SerialNumber;

        return device;
    }

}