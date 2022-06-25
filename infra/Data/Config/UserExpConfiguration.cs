using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserExpConfiguration : IEntityTypeConfiguration<UserExp>
     {
          public void Configure(EntityTypeBuilder<UserExp> builder)
          {
               builder.Property(p => p.PositionId).IsRequired();
               builder.Property(x => x.WorkedFrom).IsRequired();
               builder.Property(x => x.WorkedUpto).IsRequired();
               //builder.HasOne(p => p.Candidate).WithMany().HasForeignKey(p => p.CandidateId);
          }
     }
}