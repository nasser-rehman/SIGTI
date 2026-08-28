using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGTI.Domain.Entities;

namespace SIGTI.Infrastructure.Persistence.Configurations
{
    public sealed class TicketAssignmentConfiguration : IEntityTypeConfiguration<TicketAssignment>
    {
        public void Configure(EntityTypeBuilder<TicketAssignment> builder)
        {
            builder.ToTable("TicketAssignment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssignedAt).IsRequired();

            builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();

            builder.Property(x => x.FinishedAt);

            builder.HasIndex(x => x.TicketId);

            builder.HasIndex(x => x.TechnicianId);

            builder.HasIndex(x => x.AssignedById);

            builder.HasIndex(x => x.FinishedAt);

            builder
                .HasOne(x => x.Ticket)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Technician)
                .WithMany(x => x.AssignedTickets)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.AssignedBy)
                .WithMany(x => x.AssignmentsMade)
                .HasForeignKey(x => x.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
