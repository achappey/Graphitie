using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SecureScoresController(ILogger<SecureScoresController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<SecureScoresController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetSecureScores")]
    [Produces(typeof(IEnumerable<SecureScore>))]
    [EnableQuery]
    public async Task<IEnumerable<SecureScore>> Get()
    {
        return await _graphitieService.GetSecureScores();
    }
}
