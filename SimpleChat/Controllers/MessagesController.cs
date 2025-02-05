using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.Hubs;
using SimpleChat.Models;
using SimpleChat.Services;

namespace SimpleChat.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController(
    IMessageService messageService,
    IHubContext<ChatHub, IChatClient> hubContext,
    ILogger<MessagesController> logger) : Controller
{
    /// <summary>
    /// Sends a message and broadcasts it to all clients.
    /// </summary>
    /// <param name="messageDto">The message to send.</param>
    /// <returns>A response with the message details.</returns>
    /// <response code="200">Message successfully sent and broadcasted.</response>
    /// <response code="400">Invalid input or validation error.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
    {
        try
        {
            var message = new Message
            {
                Text = messageDto.Text,
                Timestamp = DateTime.UtcNow,
                OrderNumber = messageDto.OrderNumber
            };
        
            await Task.WhenAll(
                messageService.AddMessageToDatabaseAsync(message),
                hubContext.Clients.All.ReceiveMessage(message)
            );

            logger.LogInformation("Message with order number {OrderNumber} sent successfully.", messageDto.OrderNumber);
            return Ok(new { message.Id, message.Text, message.Timestamp });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while sending the message.");
            return StatusCode(500, "An error occurred on the server.");
        }
    }

    /// <summary>
    /// Gets messages between two specified dates.
    /// </summary>
    /// <param name="from">Start date in the format 'yyyy-MM-dd HH:mm:ss'.</param>
    /// <param name="to">End date in the format 'yyyy-MM-dd HH:mm:ss'.</param>
    /// <returns>A list of messages that were sent between the specified dates.</returns>
    /// <response code="200">Messages successfully retrieved.</response>
    /// <response code="400">Invalid date format. The correct format is 'yyyy-MM-dd HH:mm:ss'.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] string from, [FromQuery] string to)
    {
        try
        {
            if (!TryParseDate(from, out DateTime fromDate) || !TryParseDate(to, out DateTime toDate))
                return BadRequest("Invalid date format. Use 'yyyy-MM-dd HH:mm:ss'.");
            
            var messages = await messageService.GetMessagesFromDatabaseAsync(fromDate, toDate);

            logger.LogInformation("Successfully fetched {MessageCount} messages between {FromDate} and {ToDate}.", messages.Count, from, to);
            return Ok(messages);
            
            bool TryParseDate(string dateString, out DateTime date) =>
                DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while sending the message.");
            return StatusCode(500, "An error occurred on the server.");
        }
       
    }
}