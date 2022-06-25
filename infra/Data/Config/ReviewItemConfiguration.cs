using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ReviewItemConfiguration : IEntityTypeConfiguration<ReviewItem>
     {
          public void Configure(EntityTypeBuilder<ReviewItem> builder)
          {
               builder.Property(p => p.ReviewParameter).IsRequired().HasMaxLength(250);
               builder.HasIndex(p => new {p.ContractReviewItemId, p.SrNo}).IsUnique();
               builder.HasIndex(p => new {p.ContractReviewItemId, p.ReviewParameter}).IsUnique();
          }
     }
}