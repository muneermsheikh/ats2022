using core.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CandidateRawConfiguration : IEntityTypeConfiguration<ProspectiveCandidate>
     {
        public void Configure(EntityTypeBuilder<ProspectiveCandidate> builder)
        {
            builder.HasIndex(x => new {x.ResumeId, x.Source}).IsUnique();
        }
     }
}