using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.Users;

public class UpdateUserProfileRequestValidator: AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

    }
}
