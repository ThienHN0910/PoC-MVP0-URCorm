using System.ComponentModel.DataAnnotations;

namespace mvp.Interfaces;

public sealed class UpdateReplyRequest
{
    [Required]
    public string ReviewId { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string SelectedReply { get; set; } = string.Empty;
}
