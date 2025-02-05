using Microsoft.AspNetCore.Mvc;
using SimpleChat.Services;

namespace SimpleChat.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("")]
public class ChatController(
    IMessageService messageService,
    ILogger<ChatController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("last-10-minutes")]
    public async Task<IActionResult> GetMessagesLast10Minutes()
    {
        var fromDate = DateTime.UtcNow.AddMinutes(-10);
        var toDate = DateTime.UtcNow;
        
        try
        {
            var messages = await messageService.GetMessagesFromDatabaseAsync(fromDate, toDate);
            logger.LogInformation("Successfully fetched {MessageCount} messages from {FromDate} to {ToDate}.", messages.Count, fromDate, toDate);

            return View("Last10Messages", messages);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching messages from {FromDate} to {ToDate}.", fromDate, toDate);
            return StatusCode(500, "An error occurred on the server.");
        }
    }
}