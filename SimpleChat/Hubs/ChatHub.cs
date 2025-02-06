using Microsoft.AspNetCore.SignalR;
using SimpleChat.Models;
using SimpleChat.Services;

namespace SimpleChat.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger = LogHost.GetLogger<ChatHub>();
    public async Task SendMessage(Message message)
    {
        try
        {
            await Clients.Group("chat").ReceiveMessage(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the message.");
            throw;
        }
    }
}

