using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class LicensesController(ILogger<LicensesController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<LicensesController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetLicences")]
    [Produces("application/json")] 
    [EnableQuery]
    public async Task<IEnumerable<License>> Get()
    {
        return await _graphitieService.GetLicenses();
    }

}