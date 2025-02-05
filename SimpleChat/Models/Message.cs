namespace SimpleChat.Models;

public class Message
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public int OrderNumber { get; set; }
}