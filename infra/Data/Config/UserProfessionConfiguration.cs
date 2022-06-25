using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserProfessionConfiguration : IEntityTypeConfiguration<UserProfession>
     {
          public void Configure(EntityTypeBuilder<UserProfession> builder)
          {
               builder.Property(p => p.CandidateId).IsRequired();
               builder.HasIndex(p => new {p.CandidateId, p.CategoryId, p.IndustryId}).IsUnique();
               builder.HasIndex(p => p.CandidateId).HasFilter("[IsMain]=1");

               //builder.HasOne(p => p.Candidate).WithMany().HasForeignKey(p => p.CandidateId);
          }
     }
}