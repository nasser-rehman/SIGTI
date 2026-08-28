using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGTI.Domain.Entities;

namespace SIGTI.Infrastructure.Persistence.Configurations
{
    public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

            builder.Property(x => x.IsActive).IsRequired();

            builder.HasIndex(x => x.Name);

            builder
                .HasMany(x => x.Users)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Tickets)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(x => x.Users).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(x => x.Tickets).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
