using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

public static class TestDataFactory
{
    public static Ticket Create()
    {
        return new Ticket(
            number: 1,
            title: "Erro no computador",
            description: "Tela azul ao iniciar.",
            priority: TicketPriority.Medium,
            category: TicketCategory.Hardware,
            department: new DepartmentBuilder().Build(),
            createdBy: new UserBuilder().Build(),
            queue: new SupportQueueBuilder().Build()
        );
    }
}
