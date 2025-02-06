using Npgsql;
using SimpleChat.Models;

namespace SimpleChat.Services.Repositories;

public class MessageRepository(IConfiguration configuration) : IMessageRepository
{
    private readonly ILogger<MessageRepository> _logger = LogHost.GetLogger<MessageRepository>();
    private readonly string _connectionString = configuration.GetConnectionString("Database")!;

    public async Task AddMessageAsync(Message message)
    {
        const string query = $"INSERT INTO \"{Constants.MessagesTableName}\" (\"Id\", \"Text\", \"Timestamp\", \"OrderNumber\") " +
                             "VALUES (@Id, @Text, @Timestamp, @OrderNumber);";
        
        _logger.LogInformation("Adding message with parameters Text: {Text}, Timestamp: {Timestamp}, OrderNumber: {OrderNumber}", 
            message.Id, message.Text, message.Timestamp, message.OrderNumber);
        
        await ExecuteCommandAsync(query, command =>
        {
            message.Id = Guid.NewGuid();
            command.Parameters.AddWithValue("@Id", message.Id);
            command.Parameters.AddWithValue("@Text", message.Text);
            command.Parameters.AddWithValue("@Timestamp", message.Timestamp);
            command.Parameters.AddWithValue("@OrderNumber", message.OrderNumber);
        });
    }

    public async Task<List<Message>> GetMessagesAsync(DateTime fromDate, DateTime toDate)
    {
        fromDate = ConvertToUtcIfNeeded(fromDate);
        toDate = ConvertToUtcIfNeeded(toDate);

        const string query = $"SELECT \"Id\", \"Text\", \"Timestamp\", \"OrderNumber\" FROM \"{Constants.MessagesTableName}\" " +
                             "WHERE \"Timestamp\" BETWEEN @from AND @to ORDER BY \"Timestamp\";";
        
        _logger.LogInformation("Executing command with parameters FromDate: {FromDate}, ToDate: {ToDate}", fromDate, toDate);
        var messages = await ExecuteQueryAsync(query, reader => new Message
        {
            Id = reader.GetGuid(0),
            Text = reader.GetString(1),
            Timestamp = reader.GetDateTime(2),
            OrderNumber = reader.GetInt32(3)
        }, command =>
        {
            command.Parameters.AddWithValue("@from", fromDate);
            command.Parameters.AddWithValue("@to", toDate);
        });

        return messages;
        
        DateTime ConvertToUtcIfNeeded(DateTime date) =>
            date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
    }
    
    private async Task ExecuteCommandAsync(string query, Action<NpgsqlCommand> parametersSetter)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            parametersSetter(command);
            
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing the command: {Query}", query);
            throw;
        }
    }
    
    private async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<NpgsqlDataReader, T> mapper, Action<NpgsqlCommand> parametersSetter)
    {
        var result = new List<T>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            parametersSetter(command);
            
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(mapper(reader));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing query: {Query}", query);
            throw;
        }
    }
    
    public async Task EnsureDatabaseCreatedAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            
            const string createTableQuery = $"CREATE TABLE IF NOT EXISTS \"{Constants.MessagesTableName}\" (" +
                                            "\"Id\" UUID PRIMARY KEY, " +
                                            "\"Text\" TEXT NOT NULL, " +
                                            "\"Timestamp\" TIMESTAMPTZ NOT NULL, " +
                                            "\"OrderNumber\" INT NOT NULL);";

            await using var command = new NpgsqlCommand(createTableQuery, connection);
            await command.ExecuteNonQueryAsync();
        
            _logger.LogInformation("Database and table have been ensured to exist.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while ensuring the database and table exist.");
            throw;
        }
    }
}