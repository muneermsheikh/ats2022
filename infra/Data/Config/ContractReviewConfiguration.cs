using System;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ContractReviewConfiguration : IEntityTypeConfiguration<ContractReview>
     {
        public void Configure(EntityTypeBuilder<ContractReview> builder)
        {
            builder.HasIndex(x => x.OrderId).IsUnique();
            builder.HasIndex(x => x.OrderNo).IsUnique();
            builder.Property(x => x.CustomerName).IsRequired();

            builder.HasMany(s => s.ContractReviewItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
     }
}