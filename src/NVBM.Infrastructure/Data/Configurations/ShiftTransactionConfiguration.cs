using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class ShiftTransactionConfiguration : IEntityTypeConfiguration<ShiftTransaction>
{
    public void Configure(EntityTypeBuilder<ShiftTransaction> builder)
    {
        builder.HasKey(st => st.Id);
        
        builder.Property(st => st.TransactionType).IsRequired().HasMaxLength(50);
        builder.Property(st => st.Reason).HasMaxLength(500);

        builder.Property(st => st.Amount).HasColumnType("decimal(18,2)");
    }
}
