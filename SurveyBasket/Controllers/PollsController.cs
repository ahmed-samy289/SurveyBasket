using Microsoft.AspNetCore.Authorization;
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

        var Response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(Response);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute]int Id, CancellationToken cancellationToken)
    {
        var poll =await _pollService.GetAsync(Id,cancellationToken);

        if (poll is null)
        {
            return NotFound();
        }

        PollResponse response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody]PollRequest request , CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll.Adapt<PollResponse>());
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute]int Id,[FromBody]PollRequest request ,CancellationToken cancellationToken )
    {
        var IsUpdated = await _pollService.PutAsync(Id, request.Adapt<Poll>(), cancellationToken);

        if (!IsUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute]int Id,CancellationToken cancellationToken)
    {

        var IsDeleted = await _pollService.DeleteAsync(Id, cancellationToken);

        if (!IsDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{Id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute]int Id, CancellationToken cancellationToken)
    {
        var IsUpdated = await _pollService.TogglePublishAsync(Id, cancellationToken);

        if (!IsUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

}
