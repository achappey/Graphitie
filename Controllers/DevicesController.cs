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
public class DevicesController : ControllerBase
{
    private readonly ILogger<DevicesController> _logger;

    private readonly GraphitieService _graphitieService;

    public DevicesController(ILogger<DevicesController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetDevices")]
    [EnableQuery]
    public async Task<IEnumerable<Device>> Get()
    {
        return await _graphitieService.GetDevices();
    }

    [HttpDelete("{id}", Name = "DeleteDevice")]
    public IActionResult Delete(string id)
    {
        return Ok();
    }

}