namespace SurveyBasket.Errors;

public class VoteErrors
{
    //public static Error NotFound => new("Question.NotFound", "The Question was not found.");
    public static Error HasVoted => new("Vote.DuplictedContent", "This Poll Voted Before ");
}
