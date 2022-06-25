using System;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class TaskConfiguration : IEntityTypeConfiguration<ApplicationTask>
     {
          public void Configure(EntityTypeBuilder<ApplicationTask> builder)
          {
               builder.HasIndex(p => p.TaskTypeId);
               builder.HasIndex(p => p.TaskOwnerId);
               builder.HasIndex(p => p.AssignedToId);
               builder.Property(x => x.TaskDate).IsRequired().HasMaxLength(250);
               builder.HasIndex(x => new{x.AssignedToId, x.OrderItemId, x.CandidateId, x.TaskTypeId})
                    .HasFilter("CandidateId > 0") .IsUnique();

               builder.HasMany(o => o.TaskItems).WithOne().OnDelete(DeleteBehavior.Cascade);
          }
     }
}