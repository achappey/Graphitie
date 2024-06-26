using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SignInsController(ILogger<SignInsController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<SignInsController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetSignIns")]
    [Produces("application/json")] 
    [EnableQuery]
    public async Task<IEnumerable<SignIn>> Get()
    {
        return await _graphitieService.GetSignIns();
    }
}

