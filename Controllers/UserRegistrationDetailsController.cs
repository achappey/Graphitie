using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Roles = ("Administrators"))]
public class UserRegistrationDetailsController : ControllerBase
{
    private readonly ILogger<UserRegistrationDetailsController> _logger;

    private readonly GraphitieService _graphitieService;

    public UserRegistrationDetailsController(ILogger<UserRegistrationDetailsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetUserRegistrationDetails")]
    [EnableQuery]
    public async Task<IEnumerable<UserRegistrationDetails>> Get()
    {
        return await _graphitieService.GetUserRegistrationDetails();
    }
}
