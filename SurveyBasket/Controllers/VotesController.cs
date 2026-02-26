using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Errors;

namespace SurveyBasket.Controllers;
[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("")]
    public async Task<IActionResult> Start(int pollId, CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error.Equals(VoteErrors.HasVoted)
            ? result.ToProblem(StatusCodes.Status409Conflict)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }
}
