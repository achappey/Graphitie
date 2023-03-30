using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;

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
}
