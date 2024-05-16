using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class UserRegistrationDetailsController(ILogger<UserRegistrationDetailsController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<UserRegistrationDetailsController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetUserRegistrationDetails")]
    [EnableQuery]
    public async Task<IEnumerable<UserRegistrationDetails>> Get()
    {
        return await _graphitieService.GetUserRegistrationDetails();
    }
}
