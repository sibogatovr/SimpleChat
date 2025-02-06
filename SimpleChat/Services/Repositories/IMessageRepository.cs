using SimpleChat.Models;

namespace SimpleChat.Services.Repositories;

public interface IMessageRepository
{
    Task AddMessageAsync(Message message);
    Task<List<Message>> GetMessagesAsync(DateTime fromDate, DateTime toDate);
    Task EnsureDatabaseCreatedAsync();
}