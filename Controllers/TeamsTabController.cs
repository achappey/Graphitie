using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Microsoft.AspNetCore.Authorization;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ("Administrators"))]
public class TeamsTabController : ControllerBase
{
    private readonly ILogger<TeamsTabController> _logger;

    private readonly GraphitieService _graphitieService;

    public TeamsTabController(ILogger<TeamsTabController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpPost(Name = "AddTab")]
    public async Task AddTab(string groupId, string name, string url)
    {
        await _graphitieService.AddTab(groupId, name, url);

    }


}
