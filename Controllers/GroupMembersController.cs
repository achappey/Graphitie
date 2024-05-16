using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Microsoft.AspNetCore.Authorization;
using Graphitie.Models;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class GroupMembersController(ILogger<GroupMembersController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<GroupMembersController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpDelete(Name = "DeleteMember")]
    public async Task DeleteMember(string groupId, string userId)
    {
        await _graphitieService.DeleteMember(groupId, userId);

    }

}
