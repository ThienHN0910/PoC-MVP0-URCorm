using Microsoft.AspNetCore.Mvc;
using mvp.Interfaces;

namespace mvp.Controllers;

[ApiController]
[Route("api")]
public sealed class DataController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("data")]
    public async Task<ActionResult<IReadOnlyList<GetDataResponseItem>>> GetData(CancellationToken cancellationToken)
    {
        var data = await reviewService.GetDataAsync(cancellationToken);
        return Ok(data);
    }

    [HttpPut("data")]
    public async Task<IActionResult> UpdateReply([FromBody] UpdateReplyRequest request, CancellationToken cancellationToken)
    {
        var updated = await reviewService.ResolveReviewAsync(request.ReviewId, request.SelectedReply, cancellationToken);

        return updated
            ? NoContent()
            : NotFound(new { message = $"Review '{request.ReviewId}' was not found." });
    }

    [HttpPost("callAI")]
    public async Task<ActionResult<object>> CallAi([FromBody] CallAiRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var suggestions = await reviewService.GenerateAndSaveSuggestionsAsync(request.ReviewId, request.ReviewText, request.Rating, cancellationToken);
            return Ok(suggestions);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
