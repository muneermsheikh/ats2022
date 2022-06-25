using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
     {
          public void Configure(EntityTypeBuilder<Qualification> builder)
          {
               builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
               builder.HasIndex(p => p.Name).IsUnique();
          }
     }
}