using SimpleChat.Models;

namespace SimpleChat.Services;

public interface IMessageService
{
    Task AddMessageToDatabaseAsync(Message message);
    Task<List<Message>> GetMessagesFromDatabaseAsync(DateTime fromDate, DateTime toDate);
}