using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ILogger<EmployeesController> _logger;

    private readonly GraphitieService _graphitieService;

    public EmployeesController(ILogger<EmployeesController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetEmployees")]
    [EnableQuery]
    public async Task<IEnumerable<Employee>> Get()
    {
        return await _graphitieService.GetEmployees();
    }
}
