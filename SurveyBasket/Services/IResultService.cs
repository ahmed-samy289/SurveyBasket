using SurveyBasket.Contracts.Results;

namespace SurveyBasket.Services;

public interface IResultService
{
    Task<Result<PollVoteResponse>> GetPollVotesAsync(int PollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerDayReponse>>> GetVotesPerDayAsync(int PollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int PollId, CancellationToken cancellationToken = default);
}
