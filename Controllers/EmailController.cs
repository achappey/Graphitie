using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Microsoft.AspNetCore.Authorization;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ("Administrators"))]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;

    private readonly GraphitieService _graphitieService;

    public EmailController(ILogger<EmailController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpPost(Name = "SendEmail")]
    public async Task SendEmail(string sender, string recipient, string subject)
    {
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync();
            
            await _graphitieService.SendEmail(sender, recipient, subject, body);
        }

    }
}
