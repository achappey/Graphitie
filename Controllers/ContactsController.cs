using Microsoft.AspNetCore.Mvc;
using Graphitie.Models; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ILogger<ContactsController> _logger;

    private readonly GraphitieService _graphitieService;

    public ContactsController(ILogger<ContactsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "CopyMemberContacts")]
    public async Task CopyMemberContacts(string userId, string folderName, string? birthdaySiteId = null)
    {
        await _graphitieService.CopyMemberContacts(userId, folderName, birthdaySiteId);
    }

    [HttpPost(Name = "AddContacts")]
    public async Task AddContacts(string userId, string folderName, [FromBody] List<Contact> contacts)
    {
        await _graphitieService.AddContacts(userId, folderName);
    }
}
