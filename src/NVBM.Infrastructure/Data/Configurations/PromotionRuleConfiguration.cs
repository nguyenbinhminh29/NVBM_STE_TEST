using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class PromotionRuleConfiguration : IEntityTypeConfiguration<PromotionRule>
{
    public void Configure(EntityTypeBuilder<PromotionRule> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.RuleType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.RulePayload).IsRequired(); // Map to NVARCHAR(MAX) in SQL Server
    }
}
