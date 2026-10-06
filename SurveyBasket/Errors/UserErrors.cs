using SurveyBasket.Abstractions;

namespace SurveyBasket.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);
   
    public static readonly Error UserDisabled =
        new("User.UserDisabled", "User Disabled, Contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);
    
    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
    
    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "another user with the same email is already exists ", StatusCodes.Status409Conflict);
    
    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);
    
    public static readonly Error DuplictedConfirmation =
        new("User.DuplictedConfirmation", "Email confirmed before", StatusCodes.Status401Unauthorized);
}
