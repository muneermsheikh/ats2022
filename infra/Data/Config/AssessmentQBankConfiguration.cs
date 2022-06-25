using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class AssessmentQBankConfiguration : IEntityTypeConfiguration<AssessmentQBank>
     {
          public void Configure(EntityTypeBuilder<AssessmentQBank> builder)
          {
               builder.Property(x => x.CategoryId).IsRequired();
               builder.HasIndex(x => x.CategoryId).IsUnique();
          }
     }
}