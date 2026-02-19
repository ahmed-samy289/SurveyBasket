using SurveyBasket.Contracts.Question;
using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken);
        
        if (!pollIsExists)
        {
            return Result.Failure<QuestionResponse>(PollErrors.NotFound);
        }

        var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == PollId, cancellationToken);

        if (questionIsExists) { 
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedContent);
        }

        var question = request.Adapt<Question>();
        question.PollId = PollId;
        request.Answers.ForEach(Answer => question.Answers.Add(new Answer { Content = Answer}));

        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }
}
