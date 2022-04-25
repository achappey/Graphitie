

namespace Graphitie.Models;

public class Activity
{
    public DateTimeOffset DateTime { get; set; }

    public string Name { get; set; } = null!;

    public ActivityType ActivityType { get; set; }
}

public enum ActivityType {
    LANGUAGE,
    CODE,
    DEVOPS
}