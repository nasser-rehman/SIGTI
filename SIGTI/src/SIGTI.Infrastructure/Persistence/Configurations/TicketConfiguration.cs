using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGTI.Domain.Entities;

namespace SIGTI.Infrastructure.Persistence.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

            builder.Property(x => x.Description).HasMaxLength(5000).IsRequired();

            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

            builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(30);

            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(30);

            builder.Property(x => x.Number).IsRequired();

            builder.Property(x => x.DepartmentName).HasMaxLength(150).IsRequired();

            builder.Ignore(x => x.Code);

            builder.HasIndex(x => x.Number).IsUnique();
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.DepartmentId);
            builder.HasIndex(x => x.QueueId);
            builder.HasIndex(x => x.Status);

            builder
                .HasOne(x => x.Department)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedTickets)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Queue)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.QueueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Comments)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Assignments)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(x => x.Comments).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(x => x.Assignments).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
