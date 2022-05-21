using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Microsoft.AspNetCore.Authorization;
using Graphitie.Models;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ("Administrators"))]
public class GroupMembersController : ControllerBase
{
    private readonly ILogger<GroupMembersController> _logger;

    private readonly GraphitieService _graphitieService;

    public GroupMembersController(ILogger<GroupMembersController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpDelete(Name = "DeleteMember")]
    public async Task DeleteMember(string groupId, string userId)
    {
        await _graphitieService.DeleteMember(groupId, userId);

    }

}
