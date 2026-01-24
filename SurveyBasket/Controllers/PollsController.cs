using Mapster;
using Microsoft.AspNetCore.Http;
using SurveyBasket.Contracts.Requests;
using SurveyBasket.Contracts.Responses;
using SurveyBasket.Mapping;


namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();

        var Response = polls.Adapt<IEnumerable<Poll>>();

        return Ok(Response);
    }

    [HttpGet("{Id}")]
    public IActionResult Get(int Id)
    {
        var poll = _pollService.Get(Id);

        if (poll is null)
        {
            return NotFound();
        }

        PollResponse response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    [HttpPost("")]
    public IActionResult Add(CreatePollRequest request)
    {
        var newPoll = _pollService.Add(request.Adapt<Poll>());

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll);
    }

    [HttpPut("{Id}")]
    public IActionResult Update(int Id, CreatePollRequest request)
    {
        var IsUpdated = _pollService.Put(Id, request.Adapt<Poll>());

        if (!IsUpdated)
        {
            return NotFound();
        }

        return NoContent();

    }

    [HttpDelete("{Id}")]
    public IActionResult Delete(int Id)
    {

        var IsDeleted = _pollService.Delete(Id);

        if (!IsDeleted)
        {
            return NotFound();
        }

        return NoContent();

    }

}
