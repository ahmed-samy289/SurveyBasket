namespace SurveyBasket.Contracts.Results;

public record PollVoteResponse(
    string Title,
    IEnumerable<VoteResponse> Votes
);
