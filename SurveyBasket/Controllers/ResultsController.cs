using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Controllers;
[Route("api/polls/{PollId}/[controller]")]
[ApiController]
[Authorize]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;

    [HttpGet("row-data")]
    public async Task<IActionResult> PollVotes([FromRoute] int PollId, CancellationToken cancellationToken = default)
    {
        var result = await _resultService.GetPollVotesAsync(PollId, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    
    [HttpGet("votes-per-day")]
    public async Task<IActionResult> VotesPerDay([FromRoute] int PollId, CancellationToken cancellationToken = default)
    {
        var result = await _resultService.GetVotesPerDayAsync(PollId, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("votes-per-question")]
    public async Task<IActionResult> GetVotesPerQuestion(int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerQuestionAsync(pollId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
}
