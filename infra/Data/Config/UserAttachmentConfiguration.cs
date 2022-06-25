using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserAttachmentConfiguration : IEntityTypeConfiguration<UserAttachment>
     {
          public void Configure(EntityTypeBuilder<UserAttachment> builder)
          {
               builder.Property(p => p.CandidateId).IsRequired();
               //builder.Property(p => p.AttachmentType).IsRequired().HasMaxLength(10);
               //builder.Property(p => p.AttachmentUrl).IsRequired();
               //builder.HasIndex(p => p.AttachmentUrl).IsUnique();
               //builder.HasOne(p => p.Candidate).WithMany().HasForeignKey(p => p.CandidateId);
               builder.Property(p => p.url).IsRequired();
          }
     }
}