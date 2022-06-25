using core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CategoryConfiguration : IEntityTypeConfiguration<Category>
     {
          public void Configure(EntityTypeBuilder<Category> builder)
          {
               builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
               builder.HasIndex(p => p.Name).IsUnique();
          }
     }
}