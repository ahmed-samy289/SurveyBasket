using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Abstractions;
using SurveyBasket.Contracts.Polls;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    
    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);

        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute]int Id, CancellationToken cancellationToken)
    {
        var result =await _pollService.GetAsync(Id,cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var IsUpdated = await _pollService.PutAsync(Id, request, cancellationToken);

        return IsUpdated.IsSuccess ? NoContent() : IsUpdated.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] int Id, CancellationToken cancellationToken)
    {

        var IsDeleted = await _pollService.DeleteAsync(Id, cancellationToken);

        return IsDeleted.IsSuccess ? NoContent() : IsDeleted.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPut("{Id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var IsUpdated = await _pollService.TogglePublishAsync(Id, cancellationToken);

        return IsUpdated.IsSuccess ? NoContent() : IsUpdated.ToProblem(StatusCodes.Status404NotFound);
    }

}
