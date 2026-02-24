using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class PromotionActionConfiguration : IEntityTypeConfiguration<PromotionAction>
{
    public void Configure(EntityTypeBuilder<PromotionAction> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.ActionType).IsRequired().HasMaxLength(50);
        builder.Property(a => a.ActionPayload).IsRequired(); // Map to NVARCHAR(MAX) in SQL Server
    }
}
