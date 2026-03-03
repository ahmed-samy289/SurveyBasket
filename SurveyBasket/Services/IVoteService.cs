using SurveyBasket.Contracts.Vote;

namespace SurveyBasket.Services;

public interface IVoteService
{
    Task<Result> AddAsync(int PollId, string UserId, VoteRequest request, CancellationToken cancellationToken);
}
