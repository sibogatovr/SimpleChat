using Microsoft.AspNetCore.Mvc;

namespace SimpleChat.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : Controller
{
    public async Task<IActionResult> SendMessage(string text)
    {
        if (string.IsNullOrEmpty(text))
            return NoContent();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        return Ok();
    }
}