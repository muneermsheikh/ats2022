using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ReviewStatusConfiguration : IEntityTypeConfiguration<ReviewStatus>
     {
          public void Configure(EntityTypeBuilder<ReviewStatus> builder)
          {
               builder.Property(p => p.Status).IsRequired().HasMaxLength(50);
               builder.HasIndex(p => p.Status).IsUnique();
          }
     }
}