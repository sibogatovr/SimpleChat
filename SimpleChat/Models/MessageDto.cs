using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Models;

public class MessageDto
{
    [Required]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 128 characters.")]
    [Display(Name = "Message Text", Description = "The text content of the message.")]
    public string Text { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Order number must be a positive integer.")]
    [Display(Name = "Order Number", Description = "The order number of the message.")]
    public int OrderNumber { get; set; }
}