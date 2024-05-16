using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class TeamsTabController(ILogger<TeamsTabController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<TeamsTabController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpPost(Name = "AddTab")]
    public async Task AddTab(string groupId, string name, string url)
    {
        await _graphitieService.AddTab(groupId, name, url);

    }


}
