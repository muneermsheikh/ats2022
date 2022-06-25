using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserQualificationConfiguration : IEntityTypeConfiguration<UserQualification>
     {
          public void Configure(EntityTypeBuilder<UserQualification> builder)
          {
               builder.Property(p => p.CandidateId).IsRequired();
               builder.HasIndex(p => new {p.CandidateId, p.QualificationId}).IsUnique();
               builder.HasIndex(p => p.CandidateId).HasFilter("[IsMain]=1");
               //builder.HasOne(p => p.Candidate).WithMany().HasForeignKey(p => p.CandidateId);
          }
     }
}