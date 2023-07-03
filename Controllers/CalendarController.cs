using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Graph = Microsoft.Graph;
using Graphitie.Models;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ILogger<CalendarController> _logger;

    private readonly GraphitieService _graphitieService;

    public CalendarController(ILogger<CalendarController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "AddCalendarDelegate")]  
    public async Task AddCalendarDelegate(string userCalendar, string userDelegate)
    {
        await _graphitieService.AddCalendarPermisson(userCalendar, userDelegate);

    }

    [HttpPost(Name = "AddCalendarEvent")] 
    [Produces("application/json")] 
    public async Task<CalendarEvent> AddCalendarEvent(string userCalendar, [FromBody] CalendarEvent userDelegate)
    {
        return await _graphitieService.AddCalendarEvent(userCalendar, userDelegate);

    }
}
