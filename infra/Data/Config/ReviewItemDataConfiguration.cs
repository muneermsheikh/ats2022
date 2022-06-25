using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ReviewItemDataConfiguration : IEntityTypeConfiguration<ReviewItemData>
     {
          public void Configure(EntityTypeBuilder<ReviewItemData> builder)
          {
               builder.Property(p => p.ReviewParameter).IsRequired().HasMaxLength(250);
               builder.HasIndex(p => p.SrNo).IsUnique();
          }
     }
}