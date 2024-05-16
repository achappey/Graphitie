using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class GroupSettingsController(ILogger<GroupSettingsController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<GroupSettingsController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpPost(Name = "RenameGroup")]
    public async Task RenameGroup(string groupId, string name)
    {
        await _graphitieService.RenameGroup(groupId, name);

    }


}
