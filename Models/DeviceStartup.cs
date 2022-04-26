using System.ComponentModel.DataAnnotations;

namespace Graphitie.Models;

public class DeviceStartup
{

    [Key]
    public string Id { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public int CoreBootTimeInMs { get; set; }
    public int TotalBootTimeInMs { get; set; }
    public int ResponsiveDesktopTimeInMs { get; set; }
    public string? RestartCategory { get; set; }
    public string? RestartStopCode { get; set; }

}