using SurveyBasket.Contracts.Question;

namespace SurveyBasket.Mapping;

public class MappingConfigrations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest,Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(x => new Answer { Content = x }).ToList());
    }
}
