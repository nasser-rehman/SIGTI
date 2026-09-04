using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SIGTI.Application.Common.Behaviors;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Common.Services;
using SIGTI.Application.Services;
using SIGTI.Application.Services.TicketAssignment;
using SIGTI.Domain.Interfaces.Services;

namespace SIGTI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
    )
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly
            );
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly
        );

        // Services
        services.AddScoped<IEntityReferenceService, EntityReferenceService>();
        services.AddScoped<
            ITechnicianAssignmentService,
            TechnicianAssignmentService
        >();
        services.AddScoped<
            ITicketAssignmentStrategy,
            LowestUtilizationStrategy
        >();

        return services;
    }
}
