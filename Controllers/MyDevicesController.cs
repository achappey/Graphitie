using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models;
using Graphitie.Extensions;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Authorize]
[Route("[controller]")]
[Authorize(Roles = ("Administrators"))]
public class MyDevicesController : ControllerBase
{
    private readonly ILogger<MyDevicesController> _logger;

    private readonly GraphitieService _graphitieService;

    public MyDevicesController(ILogger<MyDevicesController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetMyDevices")]
    [EnableQuery]
    public async Task<IEnumerable<Device>> Get()
    {
        return await _graphitieService.GetDevicesByUser(this.HttpContext.User.GetObjectIdValue());
    }

    [HttpDelete("{id}", Name = "DeleteMyDevice")]
    public IActionResult Delete(string id)
    {
        return Ok();
    }

}