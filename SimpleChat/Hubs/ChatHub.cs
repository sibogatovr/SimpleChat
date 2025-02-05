using Microsoft.AspNetCore.SignalR;
using SimpleChat.Models;
using SimpleChat.Services;

namespace SimpleChat.Hubs;

public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(Message message)
    {
        await Clients.Group("chat").ReceiveMessage(message);
    }
}

