using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Persistence;
using System.Reflection;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddControllers();


        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(connectionString));

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
