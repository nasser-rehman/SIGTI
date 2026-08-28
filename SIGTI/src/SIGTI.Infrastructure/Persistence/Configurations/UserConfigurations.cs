using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGTI.Domain.Entities;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

            builder.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();

            builder
                .Property(x => x.Email)
                .HasConversion(email => email.Value, value => new Email(value))
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();

            builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(30).IsRequired();

            builder.Property(x => x.IsSystem).IsRequired();

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.DepartmentId);
            builder.HasIndex(x => x.Role);
            builder.HasIndex(x => x.IsActive);

            builder
                .HasOne(x => x.Department)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.CreatedTickets)
                .WithOne(x => x.CreatedBy)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.AssignedTickets)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.AssignmentsMade)
                .WithOne(x => x.AssignedBy)
                .HasForeignKey(x => x.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Comments)
                .WithOne(x => x.Author)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.QueueMemberships)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Navigation(x => x.CreatedTickets)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .Navigation(x => x.AssignedTickets)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .Navigation(x => x.AssignmentsMade)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(x => x.Comments).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .Navigation(x => x.QueueMemberships)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
