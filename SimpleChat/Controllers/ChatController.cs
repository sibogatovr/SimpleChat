using Microsoft.AspNetCore.Mvc;
using SimpleChat.Services;

namespace SimpleChat.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("")]
public class ChatController(
    IMessageService messageService) : Controller
{
    private readonly ILogger<ChatController> _logger = LogHost.GetLogger<ChatController>();
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("last-10-minutes")]
    public async Task<IActionResult> GetMessagesLast10Minutes()
    {
        const int timeIntervalInMinutes = -10;
        
        var fromDate = DateTime.UtcNow.AddMinutes(timeIntervalInMinutes);
        var toDate = DateTime.UtcNow;
        
        try
        {
            var messages = await messageService.GetMessagesFromDatabaseAsync(fromDate, toDate);

            return View("Last10Messages", messages);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while fetching messages from {FromDate} to {ToDate}.", fromDate, toDate);
            return StatusCode(500, "An error occurred on the server.");
        }
    }
}