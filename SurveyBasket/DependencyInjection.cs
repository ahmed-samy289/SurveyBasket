using Mapster;
using MapsterMapper;
using System.Reflection;
using FluentValidation.AspNetCore;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        
        services.AddControllers();
        
        services
            .AddSwaggerServices()
            .AddFluentValidationServices()
            .AddMapsterServices();


        services.AddScoped<IPollService, PollService>();

        return services;

    }

    public static IServiceCollection AddSwaggerServices (this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }
    public static IServiceCollection AddFluentValidationServices (this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddFluentValidationAutoValidation();

        return services;
    }
    public static IServiceCollection AddMapsterServices (this IServiceCollection services)
    {
        // Add Mapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(
            new Mapper(mappingConfig)
        );
        return services;
    }



}
