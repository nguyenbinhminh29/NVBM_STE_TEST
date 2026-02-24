using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
        builder.Property(p => p.StartDate).IsRequired();
        builder.Property(p => p.EndDate).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.IsStackable).IsRequired();

        builder.HasMany(p => p.Rules)
            .WithOne(r => r.Promotion)
            .HasForeignKey(r => r.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Actions)
            .WithOne(a => a.Promotion)
            .HasForeignKey(a => a.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
