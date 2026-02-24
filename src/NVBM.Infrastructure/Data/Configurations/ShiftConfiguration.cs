using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.PosDeviceId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Status).IsRequired().HasMaxLength(20);

        builder.Property(s => s.StartingCash).HasColumnType("decimal(18,2)");
        builder.Property(s => s.ExpectedCash).HasColumnType("decimal(18,2)");
        builder.Property(s => s.ActualCash).HasColumnType("decimal(18,2)");
        builder.Property(s => s.TotalSalesAmount).HasColumnType("decimal(18,2)");
        builder.Property(s => s.TotalCashReceived).HasColumnType("decimal(18,2)");

        builder.HasMany(s => s.Transactions)
            .WithOne(st => st.Shift)
            .HasForeignKey(st => st.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
