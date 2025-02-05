using Microsoft.EntityFrameworkCore;
using SimpleChat.Data;
using SimpleChat.Models;

namespace SimpleChat.Services;

public class MessageService(
    ApplicationDbContext dbContext,
    ILogger<MessageService> logger) : IMessageService
{
    public async Task AddMessageToDatabaseAsync(Message message)
    {
        try
        {
            dbContext.Messages.Add(message);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Message with ID {MessageId} successfully added to the database.", message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while adding the message to the database.");
            throw;
        }
    }

    public async Task<List<Message>> GetMessagesFromDatabaseAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            fromDate = ConvertToUtcIfNeeded(fromDate);
            toDate = ConvertToUtcIfNeeded(toDate);

            var messages = await dbContext.Messages
                .Where(m => m.Timestamp >= fromDate && m.Timestamp <= toDate)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            logger.LogInformation("Retrieved {MessageCount} messages between {FromDate} and {ToDate}.", messages.Count,
                fromDate, toDate);
            return messages;
            
            DateTime ConvertToUtcIfNeeded(DateTime date) =>
                date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving messages from the database.");
            throw;
        }
    }
}