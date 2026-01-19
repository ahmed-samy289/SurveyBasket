using Microsoft.AspNetCore.Http;


namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        return Ok(_pollService.GetAll());
    }

    [HttpGet("{Id}")]
    public IActionResult Get(int Id)
    {
        var polls = _pollService.Get(Id);

        if (polls is null)
        {
            return NotFound();
        }

        return Ok(polls);
    }

    [HttpPost("")]
    public IActionResult Add(Poll request)
    {
        var newPoll = _pollService.Add(request);

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll);
    }

    [HttpPut("{Id}")]
    public IActionResult Update(int Id, Poll request)
    {
        var IsUpdated = _pollService.Put(Id, request);

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
