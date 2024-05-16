using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class DevicesController(ILogger<DevicesController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<DevicesController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetDevices")]
    [EnableQuery]
    public async Task<IEnumerable<Device>> Get()
    {
        return await _graphitieService.GetDevices();
    }


}