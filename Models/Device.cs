using System.ComponentModel.DataAnnotations;

namespace Graphitie.Models;

public class Device
{

    [Key]
    public string Id { get; set; } = string.Empty;
    public string? OperatingSystem { get; set; }
    public string? OperatingSystemVersion { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? DeviceId { get; set; }
    public string? DisplayName { get; set; }
    public bool AccountEnabled { get; set; }
    public DateTimeOffset RegistrationDateTime { get; set; }
    public string? RegisteredOwner { get; set; }
    public string? Imei { get; set; }
    public string? SerialNumber { get; set; }
    public DateTimeOffset? LastSyncDateTime { get; set; }
    public bool? IsEncrypted { get; set; }
    public long? TotalStorageSpaceInBytes { get; set; }
    public long? FreeStorageSpaceInBytes { get; set; }


}