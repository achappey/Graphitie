using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Microsoft.AspNetCore.Authorization;
using Graphitie.Models;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ("Administrators"))]
public class GroupOwnersController : ControllerBase
{
    private readonly ILogger<GroupOwnersController> _logger;

    private readonly GraphitieService _graphitieService;

    public GroupOwnersController(ILogger<GroupOwnersController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

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
