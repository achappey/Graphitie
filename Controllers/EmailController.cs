using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;
using Graphitie.Models;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;

    private readonly GraphitieService _graphitieService;

    public EmailController(ILogger<EmailController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpPut(Name = "Email")]  
    public async Task SendMail(string user, [FromBody] Mail email)
    {
        await _graphitieService.SendMail(user, email);
    }

    [HttpPost(Name = "SendEmail")]  
    public async Task SendEmail(string user, string sender, string recipient, string subject, [FromBody] Email email)
    {
        await _graphitieService.SendEmail(user, sender, recipient, subject, email.Html!);

    }
}
