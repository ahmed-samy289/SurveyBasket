namespace SurveyBasket.Abstractions.Consts;

public static class Permissions
{
    public static string Type { get; set; } = "Permissions";

    public const string GetPolls = "poll:read";
    public const string AddPolls = "poll:add";
    public const string UpdatePolls = "poll:update";
    public const string DeletePolls = "poll:delete";

    public const string GetQuestions = "question:read";
    public const string AddQuestions = "question:add";
    public const string UpdateQuestions = "question:update";

    public const string GetUsers = "user:read";
    public const string AddUsers = "user:add";
    public const string UpdateUsers = "user:update";

    public const string GetRoles = "role:read";
    public const string AddRoles = "role:add";
    public const string UpdateRoles = "role:update";

    public const string Results = "result:read";

    public static IList<string?> GetAllPermissions() => 
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}
