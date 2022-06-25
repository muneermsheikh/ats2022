using System;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ContractReviewItemConfiguration : IEntityTypeConfiguration<ContractReviewItem>
     {
        public void Configure(EntityTypeBuilder<ContractReviewItem> builder)
        {
            /* builder.Property(s => s.ReviewItemStatus).HasConversion(
                o => o.ToString(),
                o => (EnumReviewItemStatus) Enum.Parse(typeof(EnumReviewItemStatus), o)
            );
            */
            builder.HasIndex(x => x.OrderItemId).IsUnique();
            builder.HasMany(x => x.ReviewItems).WithOne().OnDelete(DeleteBehavior.Cascade);

        }
     }
}