using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SurveyBasket.Contracts.Question;

namespace SurveyBasket.Services;

public interface IQuestionService
{
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken);
    Task<Result<QuestionResponse>> GetAsync(int PollId, int Id, CancellationToken cancellationToken);
    Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int PollId, int Id, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int PollId, int Id, CancellationToken cancellationToken = default);
}
