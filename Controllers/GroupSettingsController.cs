using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Microsoft.AspNetCore.Authorization;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ("Administrators"))]
public class GroupSettingsController : ControllerBase
{
    private readonly ILogger<GroupSettingsController> _logger;

    private readonly GraphitieService _graphitieService;

    public GroupSettingsController(ILogger<GroupSettingsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpPost(Name = "RenameGroup")]
    public async Task RenameGroup(string groupId, string name)
    {
        await _graphitieService.RenameGroup(groupId, name);

    }


}
