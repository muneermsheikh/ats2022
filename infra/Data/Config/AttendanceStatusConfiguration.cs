using core.Entities;
using core.Entities.HR;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class AttendanceStatusConfiguration : IEntityTypeConfiguration<InterviewAttendanceStatus>
     {
          public void Configure(EntityTypeBuilder<InterviewAttendanceStatus> builder)
          {
               builder.Property(p => p.Status).IsRequired().HasMaxLength(100);
               builder.HasIndex(p => p.Status).IsUnique();
          }
     }
}