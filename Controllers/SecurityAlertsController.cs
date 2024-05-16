using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SecurityAlertsController(ILogger<SecurityAlertsController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<SecurityAlertsController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetSecurityAlerts")]
    [Produces("application/json")] 
    [EnableQuery]
    public async Task<IEnumerable<SecurityAlert>> Get()
    {
        return await _graphitieService.GetSecurityAlerts();
    }
}
