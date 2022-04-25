using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;
using Graphitie.Extensions;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize]
public class MySignInsController : ControllerBase
{
    private readonly ILogger<MySignInsController> _logger;

    private readonly GraphitieService _graphitieService;

    public MySignInsController(ILogger<MySignInsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetMySignIns")]
    [EnableQuery]
    public async Task<IEnumerable<SignIn>> Get()
    {
        return await _graphitieService.GetSignInsByUser(this.HttpContext.User.GetObjectIdValue());
    }
}

