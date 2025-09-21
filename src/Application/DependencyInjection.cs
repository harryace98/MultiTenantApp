using Application.Features.Login;
using Application.Features.User.Command;
using Application.Features.User.Query;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        
        // Register User handlers
        services.AddScoped<UserCommandHandler>();
        services.AddScoped<UserQueryHandler>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<LoginQueryHandler>();
        
        return services;
    }
}
