using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class SkillConfiguration : IEntityTypeConfiguration<SkillData>
     {
          public void Configure(EntityTypeBuilder<SkillData> builder)
          {
               builder.Property(p => p.SkillName).IsRequired().HasMaxLength(100);
               builder.HasIndex(p => p.SkillName).IsUnique();
          }
     }
}