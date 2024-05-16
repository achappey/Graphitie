using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class EmployeesController(ILogger<EmployeesController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<EmployeesController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "GetEmployees")]
    [EnableQuery]
    public async Task<IEnumerable<Employee>> Get()
    {
        return await _graphitieService.GetEmployees();
    }
}
