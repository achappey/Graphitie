using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Graphitie.Models;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    private readonly GraphitieService _graphitieService;

    public UsersController(ILogger<UsersController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetUsers")]
    [Produces("application/json")] 
    [EnableQuery]
    public async Task<IEnumerable<User>> Get()
    {
        return await _graphitieService.GetUsers();
    }
}
