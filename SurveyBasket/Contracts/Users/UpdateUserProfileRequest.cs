namespace SurveyBasket.Contracts.Users;

public record UpdateUserProfileRequest(
    string FirstName,
    string LastName
);
