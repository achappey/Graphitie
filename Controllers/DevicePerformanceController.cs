using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class DevicePerformanceController(ILogger<DevicePerformanceController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<DevicePerformanceController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetDevicePerformance")]
    [EnableQuery]
    public async Task<IEnumerable<DevicePerformance>> Get()
    {
        return await _graphitieService.GetDevicePerformance();
    }

}