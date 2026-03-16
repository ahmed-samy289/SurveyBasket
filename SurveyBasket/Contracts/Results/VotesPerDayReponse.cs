namespace SurveyBasket.Contracts.Results;

public record VotesPerDayReponse(
    DateOnly Date,
    int NumberOfVotes
);
