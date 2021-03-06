
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Roles = ("Administrators"))]
public class SecurityAlertsController : ControllerBase
{
    private readonly ILogger<SecurityAlertsController> _logger;

    private readonly GraphitieService _graphitieService;

    public SecurityAlertsController(ILogger<SecurityAlertsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetSecurityAlerts")]
    [EnableQuery]
    public async Task<IEnumerable<SecurityAlert>> Get()
    {
        return await _graphitieService.GetSecurityAlerts();
    }
}
