namespace Graphitie.Models;

public class CalendarEvent
{

    public string Subject { get; set; } = null!;
    public ItemBody Body { get; set; } = null!;
    public EventDateTime Start { get; set; } = null!;
    public EventDateTime End { get; set; } = null!;
    public IEnumerable<EventAttendee> Attendees { get; set; } = null!;
    public bool IsOnlineMeeting { get; set; }
    public string? OnlineMeetingProvider { get; set; }
}

public class EventDateTime
{
    public string DateTime { get; set; } = null!;
    public string TimeZone { get; set; } = null!;
}

public class ItemBody
{
    public string ContentType { get; set; } = null!;
    public string Content { get; set; } = null!;
}

public class EventAttendee
{
    public EmailAddress EmailAddress { get; set; } = null!;
    public string Type { get; set; } = null!;
}

public class EmailAddress
{
    public string Address { get; set; } = null!;
    public string? Name { get; set; }
}
