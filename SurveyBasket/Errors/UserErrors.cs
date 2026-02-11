using SurveyBasket.Abstractions;

namespace SurveyBasket.Errors;

public class UserErrors
{
    public static Error InvalidCredentials => new("User.InvalidCredentials", "The provided email or password is incorrect.");
}
