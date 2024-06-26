using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class GroupOwnersController(ILogger<GroupOwnersController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<GroupOwnersController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpPost(Name = "AddOwner")]
    public async Task AddOwner(string groupId, string userId)
    {
        await _graphitieService.AddOwner(groupId, userId);

    }

    [HttpDelete(Name = "DeleteOwner")]
    public async Task DeleteOwner(string groupId, string userId)
    {
        await _graphitieService.DeleteOwner(groupId, userId);

    }

}
