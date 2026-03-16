using SurveyBasket.Contracts.Results;
using SurveyBasket.Entities;
using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVoteResponse>> GetPollVotesAsync(int PollId,CancellationToken cancellationToken = default)
    {
        var pollVotes = await _context.Polls
            .Where(p=>p.Id == PollId)
            .Select(x=> new PollVoteResponse(
                x.Title,
                x.Votes.Select(v=> new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(a=> new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content
                    ))
                ))
            )).SingleOrDefaultAsync(cancellationToken);

        return pollVotes is null 
            ? Result.Failure<PollVoteResponse>(PollErrors.NotFound) 
            : Result.Success(pollVotes);

    }

    public async Task<Result<IEnumerable<VotesPerDayReponse>>> GetVotesPerDayAsync(int PollId , CancellationToken cancellationToken = default)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);

        if (!isPollExist)
        {
            return Result.Failure<IEnumerable<VotesPerDayReponse>>(PollErrors.NotFound);
        }

        var votesPerDay = await _context.Votes
            .Where(x => x.PollId == PollId)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
            .Select(g => new VotesPerDayReponse(
                g.Key.Date,
                g.Count()
            ))
            .ToListAsync(cancellationToken);
        
        return Result.Success<IEnumerable<VotesPerDayReponse>>(votesPerDay);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int PollId , CancellationToken cancellationToken = default)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);

        if (!isPollExist)
        {
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.NotFound);
        }

        var votesPerDay = await _context.VoteAnswers
            .Where(x=>x.Vote.PollId == PollId)
            .Select(x => new VotesPerQuestionResponse(
                x.Question.Content,
                x.Question.Votes
                    .GroupBy(x=> new { AnswerId = x.Answer.Id , AnswerContent = x.Answer.Content })
                    .Select(g => new VotesPerAnswerResponse(
                        g.Key.AnswerContent,
                        g.Count()
                    ))
            )).ToListAsync(cancellationToken);

        //var result = await _context.Questions
        //.Where(q => q.PollId == pollId)
        //.Select(q => new VotesPerQuestionResponse(
        //    q.Content,
        //    q.Answers
        //        .Select(a => new VotesPerAnswerResponse(
        //            a.Content,
        //            a.Votes.Count()
        //        ))
        //))
        //.ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerDay);
    }

}
