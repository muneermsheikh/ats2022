using System;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
     {
          public void Configure(EntityTypeBuilder<OrderItem> builder)
          {
               /*
               builder.Property(s => s.Status).HasConversion(
               o => o.ToString(),
               o => (int)(EnumOrderItemStatus) Enum.Parse(typeof(EnumOrderItemStatus), o)
               );
               */
               
               builder.HasOne(x => x.JobDescription).WithOne().OnDelete(DeleteBehavior.Cascade);
               builder.HasOne(s => s.Remuneration).WithOne().OnDelete(DeleteBehavior.Cascade);
               builder.HasMany(s => s.CVRefs).WithOne().HasForeignKey(x => x.OrderItemId) .OnDelete(DeleteBehavior.Cascade);

               //builder.HasOne(x => x.CVRefs).HasForeignKey(x => x.OrderItemId);
               //e.HasOne(x => x.ImageBackground).WithOne(x => x.ImageBackground).HasForeignKey<Publication>(x => x.ImageBackgroundId).IsRequired(false)
          }
     }
}