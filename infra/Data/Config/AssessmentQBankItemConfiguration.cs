using core.Entities;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class AssessmentQBankItemConfiguration : IEntityTypeConfiguration<AssessmentQBankItem>
     {
          public void Configure(EntityTypeBuilder<AssessmentQBankItem> builder)
          {
               builder.Property(p => p.QNo).IsRequired();
               builder.Property(p => p.AssessmentParameter).IsRequired();
               builder.Property(p => p.Question).IsRequired();
               builder.HasIndex(p => new {p.AssessmentQBankId, p.QNo}).IsUnique();

               //builder.Property(p => p.Question).HasMaxLength(200).IsRequired().HasMaxLength(50);
          }
     }
}