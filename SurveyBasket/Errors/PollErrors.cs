namespace SurveyBasket.Errors;

public class PollErrors
{
    public static Error NotFound => new("Poll.NotFound", "The poll was not found.", StatusCodes.Status404NotFound);
    public static Error DuplicatedTitle => new("Poll.DuplictedTitle", "The poll title is already exists.", StatusCodes.Status409Conflict);

}
