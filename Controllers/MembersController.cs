using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class MembersController : ControllerBase
{
    private readonly ILogger<MembersController> _logger;

    private readonly GraphitieService _graphitieService;

    public MembersController(ILogger<MembersController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetMembers")]
    [EnableQuery]
    public async Task<IEnumerable<User>> Get()
    {
        return await _graphitieService.GetMembers();
    }
}
