using System.ComponentModel.DataAnnotations;

namespace mvp.Interfaces;

public sealed class CallAiRequest
{
    [Required]
    public string ReviewId { get; set; } = string.Empty;

    [Required]
    public string ReviewText { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }
}
