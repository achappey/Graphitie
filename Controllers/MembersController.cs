using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class MembersController(ILogger<MembersController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<MembersController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetMembers")]
    [Produces("application/json")] 
    [EnableQuery]
    public async Task<IEnumerable<User>> Get()
    {
        return await _graphitieService.GetMembers();
    }
}
