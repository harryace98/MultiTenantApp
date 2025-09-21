using MultiTenantApp.API.Infrastructure;

namespace MultiTenantApp.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddSwaggerGen();

      
        services.AddControllers()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
