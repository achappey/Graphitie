using System.ComponentModel.DataAnnotations;

namespace Graphitie.Models;

public class DeviceStartupProcess
{

    [Key]
    public string Id { get; set; } = string.Empty;
    public string? ManagedDeviceId { get; set; }
    public int StartupImpactInMs { get; set; }
    public string? ProcessName { get; set; }
    public string? ProductName { get; set; }

}