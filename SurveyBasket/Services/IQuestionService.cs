using SurveyBasket.Contracts.Question;

namespace SurveyBasket.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default);
}
