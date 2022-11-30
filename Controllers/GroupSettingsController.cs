using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
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
