using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SignInsController : ControllerBase
{
    private readonly ILogger<SignInsController> _logger;

    private readonly GraphitieService _graphitieService;

    public SignInsController(ILogger<SignInsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetSignIns")]
    [EnableQuery]
    public async Task<IEnumerable<SignIn>> Get()
    {
        return await _graphitieService.GetSignIns();
    }
}

