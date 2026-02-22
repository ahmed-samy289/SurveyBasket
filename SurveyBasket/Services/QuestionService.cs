using SurveyBasket.Contracts.Answer;
using SurveyBasket.Contracts.Question;
using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken);

        if (!pollIsExists)
        {
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.NotFound);
        }

        var questions = await _context.Questions.Where(x => x.PollId == PollId)
            .Include(x => x.Answers)
            //.Select(x => new QuestionResponse(
            //    x.Id,
            //    x.Content,
            //    x.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
            //))
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int PollId, int Id, CancellationToken cancellationToken)
    {
        var question = await _context.Questions.Where(x => x.PollId == PollId && x.Id == Id)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
        {
            return Result.Failure<QuestionResponse>(QuestionErrors.NotFound);
        }

        return Result.Success<QuestionResponse>(question);
    }

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
        //request.Answers.ForEach(Answer => question.Answers.Add(new Answer { Content = Answer}));

        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int PollId, int Id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExists = await _context.Questions.AnyAsync(
            x => x.PollId == PollId
            && x.Id != Id
            && x.Content == request.Content
            , cancellationToken
        );

        if (questionIsExists)
        {
            return Result.Failure(QuestionErrors.DuplicatedContent);
        }

        // select question
        var question = await _context.Questions.Include(x => x.Answers).SingleOrDefaultAsync(x => x.PollId == PollId && x.Id == Id, cancellationToken);

        if (question is null)
        {
            return Result.Failure(QuestionErrors.NotFound);
        }

        question.Content = request.Content;

        // current answers
        var currentAnswers = question.Answers.Select(x => x.Content).ToList();

        // new answers
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        //foreach (var item in newAnswers)
        //{
        //    question.Answers.Add(new Answer { Content = item });
        //}
        newAnswers.ForEach(Answer => question.Answers.Add(new Answer { Content = Answer }));

        //foreach (var item in question.Answers)
        //{
        //    item.isActive = request.Answers.Contains(item.Content);
        //}
        question.Answers.ToList().ForEach(a =>
        {
            a.isActive = request.Answers.Contains(a.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }

    public async Task<Result> ToggleStatusAsync(int PollId, int Id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == PollId && x.Id == Id, cancellationToken);

        if (question is null)
        {
            return Result.Failure(QuestionErrors.NotFound);
        }

        question.isActive = !question.isActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
