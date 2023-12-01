using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase
{
    private readonly ILogger<GroupsController> _logger;

    private readonly GraphitieService _graphitieService;

    public GroupsController(ILogger<GroupsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetGroups")]
    [EnableQuery]
    public async Task<IEnumerable<Group>> Get()
    {
        return await _graphitieService.GetGroups();
    }

    [HttpGet("{id}", Name = "GetSharePointUrl")]
    [EnableQuery]
    public async Task<string> GetUrl(string id)
    {
        return await _graphitieService.GetSharePointUrlOfGroup(id);
    }

}