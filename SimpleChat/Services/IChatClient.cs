using SimpleChat.Models;

namespace SimpleChat.Services;

public interface IChatClient
{
    public Task ReceiveMessage(Message message);
}