using core.Entities.Admin;
using core.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class SelectionDecisionConfiguration : IEntityTypeConfiguration<SelectionDecision>
     {
        public void Configure(EntityTypeBuilder<SelectionDecision> builder)
        {
            builder.HasIndex(x => x.CVRefId).IsUnique();
            builder.HasOne(p => p.CVRef).WithOne().OnDelete(DeleteBehavior.Restrict);
            //builder.HasOne(p => p.Employment).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
     }
}