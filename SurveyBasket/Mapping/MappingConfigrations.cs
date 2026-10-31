using SurveyBasket.Contracts.Question;
using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Mapping;

public class MappingConfigrations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest,Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(x => new Answer { Content = x }).ToList());

        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(des => des.UserName, src => src.Email);

        config.NewConfig<(ApplicationUser user,IList<string> roles),UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(des => des.UserName, src => src.Email)
            .Map(des => des.EmailConfirmed, src => true);
    }
}
