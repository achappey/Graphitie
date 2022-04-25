
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;
using Graphitie.Extensions;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ("Administrators,Users"))]
public class MySecurityAlertsController : ControllerBase
{
    private readonly ILogger<MySecurityAlertsController> _logger;

    private readonly GraphitieService _graphitieService;

    public MySecurityAlertsController(ILogger<MySecurityAlertsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetMySecurityAlerts")]
    [EnableQuery]
    public async Task<IEnumerable<SecurityAlert>> Get()
    {
        return await _graphitieService.GetSecurityAlertsByUser(this.HttpContext.GetUserPrincipalName());
    }
}
