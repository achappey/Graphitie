using System.ComponentModel.DataAnnotations;

namespace Graphitie.Models;

public class DevicePerformance
{

    [Key]
    public string Id { get; set; } = string.Empty;
    public string? DiskType { get; set; }
    public string? OperatingSystemVersion { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? DeviceName { get; set; }
    public int BootScore { get; set; }
    public string? HealthStatus { get; set; }
    public int LoginScore { get; set; }
    public int BlueScreenCount { get; set; }
    public int RestartCount { get; set; }
    public double AverageBlueScreens { get; set; }
    public double AverageRestarts { get; set; }
    public double StartupPerformanceScore { get; set; }

}