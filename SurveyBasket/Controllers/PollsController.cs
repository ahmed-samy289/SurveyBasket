using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Abstractions;
using SurveyBasket.Contracts.Polls;
using SurveyBasket.Errors;

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
        return Ok(await _pollService.GetAllAsync(cancellationToken));
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetCurrentAsync(cancellationToken));
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
        var result = await _pollService.AddAsync(request, cancellationToken);

        return result.IsSuccess 
            ? CreatedAtAction(nameof(Get), new { Id = result.Value.Id }, result.Value)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.PutAsync(Id, request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(PollErrors.DuplicatedTitle)
                ? result.ToProblem(StatusCodes.Status409Conflict)
                : result.ToProblem(StatusCodes.Status404NotFound);
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
