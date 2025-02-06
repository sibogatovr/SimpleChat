using SimpleChat.Models;
using SimpleChat.Services.Repositories;

namespace SimpleChat.Services;

public class MessageService(
    IMessageRepository messageRepository) : IMessageService
{
    private readonly ILogger<MessageService> _logger = LogHost.GetLogger<MessageService>();
    
    public async Task AddMessageToDatabaseAsync(Message message)
    {
        try
        {
            await messageRepository.AddMessageAsync(message);
            _logger.LogInformation("Message with ID {MessageId} successfully added to the database.", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the message to the database.");
            throw;
        }
    }

    public async Task<List<Message>> GetMessagesFromDatabaseAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var messages = await messageRepository.GetMessagesAsync(fromDate, toDate);
            _logger.LogInformation("Retrieved {MessageCount} messages between {FromDate} and {ToDate}.", 
                messages.Count, fromDate, toDate);
            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving messages from the database.");
            throw;
        }
    }
}