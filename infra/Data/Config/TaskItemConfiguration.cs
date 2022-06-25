using System;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
     {
          public void Configure(EntityTypeBuilder<TaskItem> builder)
          {
               builder.HasIndex(p => p.ApplicationTaskId);
               builder.Property(x => x.TransactionDate).IsRequired();
               builder.Property(x => x.TaskStatus).IsRequired();
               builder.Property(x => x.UserId).IsRequired();
               builder.HasIndex(x => x.UserId).HasFilter("[UserId] > 0");
               /*
               giving error:
                    Filtered index 'IX_TaskItems_UserId' cannot be created on table 'TaskItems' because the column 'UserId' 
                    in the filter expression is compared with a constant of higher data type precedence or of a different collation. 
                    Converting a column to the data type of a constant is not supported for filtered indexes. 
                    To resolve this error, explicitly convert the constant to the same data type and collation as the column 'UserId'.
               */

               builder.HasIndex(p => p.TransactionDate);
               builder.Property(x => x.TaskItemDescription).IsRequired().HasMaxLength(250);
               //builder.HasOne(p => p.ApplicationTask).WithMany().HasForeignKey(p => p.ApplicationTaskId);

          }
     }
}