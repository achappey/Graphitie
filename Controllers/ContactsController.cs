using Microsoft.AspNetCore.Mvc;
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class ContactsController(ILogger<ContactsController> logger, GraphitieService graphitieService) : ControllerBase
{
    private readonly ILogger<ContactsController> _logger = logger;

    private readonly GraphitieService _graphitieService = graphitieService;

    [HttpGet(Name = "CopyMemberContacts")]
    public async Task CopyMemberContacts(string userId, string folderName, string? birthdaySiteId = null)
    {
        await _graphitieService.CopyMemberContacts(userId, folderName, birthdaySiteId);
    }

    [HttpPost(Name = "AddContacts")]
    public void AddContacts(string userId, string folderName)
    {
        _graphitieService.AddContacts(userId, folderName);
    }
}
