using System;
using core.Entities.HR;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CandidateAssessmentConfiguration : IEntityTypeConfiguration<CandidateAssessment>
     {
          public void Configure(EntityTypeBuilder<CandidateAssessment> builder)
          {
               builder.Property(s => s.AssessResult).HasConversion(
               o => o.ToString(),
               o => (EnumCandidateAssessmentResult) Enum.Parse(typeof(EnumCandidateAssessmentResult), o)
               );

               builder.HasIndex(x => new {x.CandidateId, x.OrderItemId}).IsUnique();
          
               builder.HasMany(s => s.CandidateAssessmentItems).WithOne().OnDelete(DeleteBehavior.Cascade);
               //builder.HasOne(s => s.Category).WithOne().OnDelete(DeleteBehavior.Restrict);
          }
     }
}