namespace SurveyBasket.Errors;

public class QuestionErrors
{
    public static Error NotFound => new("Question.NotFound", "The Question was not found.", StatusCodes.Status404NotFound);
    public static Error DuplicatedContent => new("Question.DuplictedContent", "The Question content is already exists.", StatusCodes.Status409Conflict);
}
