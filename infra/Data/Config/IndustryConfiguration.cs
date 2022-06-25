using core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class IndustryConfiguration : IEntityTypeConfiguration<Industry>
     {
          public void Configure(EntityTypeBuilder<Industry> builder)
          {
               builder.HasIndex(p => p.Name).IsUnique();
               builder.Property(x => x.Name).IsRequired().HasMaxLength(60);
          }
     }
}