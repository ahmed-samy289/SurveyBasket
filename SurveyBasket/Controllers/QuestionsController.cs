using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Contracts.Question;
using SurveyBasket.Errors;
using System.Threading.Tasks;

namespace SurveyBasket.Controllers;
[Route("api/polls/{PollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromRoute] int PollId,CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAllAsync(PollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] int PollId,[FromRoute] int Id, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAsync(PollId, Id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int PollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.AddAsync(PollId, request, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(Get), new { PollId, Id = result.Value.Id }, result.Value);
        }

        return result.ToProblem();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int PollId, [FromRoute] int Id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(PollId, Id, request, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.ToProblem();
    }

    [HttpPut("{Id}/toggleStatus")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int PollId, [FromRoute] int Id, CancellationToken cancellationToken)
    {
        var IsToggled = await _questionService.ToggleStatusAsync(PollId, Id, cancellationToken);

        return IsToggled.IsSuccess ? NoContent() : IsToggled.ToProblem();
    }

}
