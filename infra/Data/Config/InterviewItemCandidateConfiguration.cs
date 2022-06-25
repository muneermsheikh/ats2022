using core.Entities;
using core.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class InterviewItemCandidateConfiguration : IEntityTypeConfiguration<InterviewItemCandidate>
     {
          public void Configure(EntityTypeBuilder<InterviewItemCandidate> builder)
          {
               builder.HasIndex(x => x.InterviewItemId);
               builder.HasIndex(p => new {p.CandidateId, p.InterviewItemId} ).IsUnique();
               builder.HasIndex(x => x.ApplicationNo);
          }
     }
}