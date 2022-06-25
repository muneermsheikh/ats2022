using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserPassportConfiguration : IEntityTypeConfiguration<UserPassport>
     {
          public void Configure(EntityTypeBuilder<UserPassport> builder)
          {
               builder.Property(p => p.PassportNo).IsRequired().HasMaxLength(15);
               builder.HasIndex(p => p.PassportNo).IsUnique();

               builder.HasIndex(x => x.CandidateId).HasFilter("[IsValid]=1");  //TODO - or should it be=True?
               //builder.HasOne(p => p.Candidate).WithMany().HasForeignKey(p => p.CandidateId);
          }
     }
}