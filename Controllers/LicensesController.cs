using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class LicensesController : ControllerBase
{
    private readonly ILogger<LicensesController> _logger;

    private readonly GraphitieService _graphitieService;

    public LicensesController(ILogger<LicensesController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetLicences")]
    [Produces("application/json")] 
    [EnableQuery]
    public async Task<IEnumerable<License>> Get()
    {
        return await _graphitieService.GetLicenses();
    }

}