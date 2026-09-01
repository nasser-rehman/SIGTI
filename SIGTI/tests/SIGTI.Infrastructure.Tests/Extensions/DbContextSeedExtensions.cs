using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Context;

namespace SIGTI.Infrastructure.Tests.Extensions
{
    public record BasicTicketContext(
        Department Department,
        SupportQueue Queue,
        User CreatedBy,
        User Technician
    );

    public static class DbContextSeedExtensions
    {
        public static async Task<BasicTicketContext> SeedBasicTicketContextAsync(
            this ApplicationDbContext context,
            CancellationToken cancellationToken = default
        )
        {
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();
            var createdBy = new UserBuilder()
                .WithEmail("createdBy@sigti.local")
                .WithDepartment(department)
                .Build();
            var technician = new UserBuilder()
                .WithEmail("tech@sigti.local")
                .WithDepartment(department)
                .Build();

            await context.Departments.AddAsync(department, cancellationToken);
            await context.SupportQueues.AddAsync(queue, cancellationToken);
            await context.Users.AddRangeAsync(
                new[] { createdBy, technician },
                cancellationToken
            );
            await context.SaveChangesAsync(cancellationToken);

            return new BasicTicketContext(
                department,
                queue,
                createdBy,
                technician
            );
        }
    }
}
