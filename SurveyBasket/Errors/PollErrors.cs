namespace SurveyBasket.Errors;

public class PollErrors
{
    public static Error NotFound => new("Poll.NotFound", "The poll was not found.");
}
