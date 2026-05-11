namespace SurveyBasket.Contracts.Users;

public record GetUserProfileResponse(
    string UserName,
    string Email,
    string FirstName,
    string LastName
);
