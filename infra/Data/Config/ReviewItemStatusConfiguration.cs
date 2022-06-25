using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ReviewItemStatusConfiguration : IEntityTypeConfiguration<ReviewItemStatus>
     {
          public void Configure(EntityTypeBuilder<ReviewItemStatus> builder)
          {
               builder.Property(p => p.ItemStatus).IsRequired().HasMaxLength(50);
               builder.HasIndex(p => p.ItemStatus).IsUnique();
          }
     }
}