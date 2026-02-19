namespace SurveyBasket.Contracts.Question;

public class QuestionRequestValidator: AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().Length(3, 1000);

        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1)
            .WithMessage("Question should have more than one answer");
        
        RuleFor(x => x.Answers)
            .NotNull()
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("Question cannot have duplicated answers");
    }
}
