using core.Entities;
using core.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
     {
          public void Configure(EntityTypeBuilder<Interview> builder)
          {
               builder.HasIndex(p => p.OrderId).IsUnique();
               builder.HasIndex(x => x.OrderNo).IsUnique();
               builder.Property(x => x.InterviewVenue).IsRequired().HasMaxLength(100);
               builder.Property(x => x.InterviewMode).IsRequired().HasMaxLength(50);
               builder.Property(x => x.InterviewDateFrom).IsRequired();
               builder.Property(x => x.InterviewDateUpto).IsRequired();
          }
     }
}