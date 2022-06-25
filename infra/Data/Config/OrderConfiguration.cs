using System;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class OrderConfiguration : IEntityTypeConfiguration<Order>
     {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(s => s.Status).HasConversion(
                o => o.ToString(),
                o => (EnumOrderStatus) Enum.Parse(typeof(EnumOrderStatus), o)
            );

            builder.Property(x => x.CustomerId).IsRequired();
            builder.HasIndex(x => x.CustomerId);
            builder.Property(x => x.CityOfWorking).IsRequired();

            /* builder.OwnsOne(i => i.OrderAddress, 
                io => { io.WithOwner(); }
            );
            */
            builder.HasMany(s => s.OrderItems).WithOne().IsRequired().OnDelete(DeleteBehavior.Cascade);
            //builder.HasOne(s => s.Customer).WithOne().IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
     }
}