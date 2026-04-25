namespace SurveyBasket.Contracts.Authentication;

public class ResendComfirmationEmailRequestValidator : AbstractValidator<ResendComfirmationEmailRequest>
{
    public ResendComfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        
    }

}
