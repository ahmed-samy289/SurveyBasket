using Microsoft.EntityFrameworkCore;
using SurveyBasket.Contracts.Question;
using SurveyBasket.Contracts.Vote;
using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public  async Task<Result> AddAsync(int PollId, string UserId, VoteRequest request, CancellationToken cancellationToken)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == PollId && x.UserId == UserId, cancellationToken);

        if (hasVote)
        {
            return Result.Failure(VoteErrors.DuplicatedVote);
        }

        var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == PollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExist)
        {
            return Result.Failure(PollErrors.NotFound);
        }

        var QuestionIds = await _context.Questions
            .Where(x => x.PollId == PollId && x.isActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(QuestionIds))
        {
            return Result.Failure(VoteErrors.InvalidQuestions);
        }
        
        var Vote  = new Vote
        {
            PollId = PollId,
            UserId = UserId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(Vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
