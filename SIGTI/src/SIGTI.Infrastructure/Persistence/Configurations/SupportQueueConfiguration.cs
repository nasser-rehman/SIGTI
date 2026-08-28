using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGTI.Domain.Entities;

namespace SIGTI.Infrastructure.Persistence.Configurations;

public sealed class SupportQueueConfiguration : IEntityTypeConfiguration<SupportQueue>
{
    public void Configure(EntityTypeBuilder<SupportQueue> builder)
    {
        builder.ToTable("SupportQueues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder
            .HasMany(x => x.Members)
            .WithOne(x => x.SupportQueue)
            .HasForeignKey(x => x.SupportQueueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.Tickets)
            .WithOne(x => x.Queue)
            .HasForeignKey(x => x.QueueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Members).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Tickets).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
