using System;
using core.Entities.HR;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ChecklistItemHRConfiguration : IEntityTypeConfiguration<ChecklistHRItem>
     {
        public void Configure(EntityTypeBuilder<ChecklistHRItem> builder)
        {
            builder.Property(x => x.SrNo).IsRequired();
            builder.Property(x => x.Parameter).IsRequired().HasMaxLength(200);            

            //builder.HasOne(p => p.ChecklistHR).WithMany().HasForeignKey(p => p.ChecklistHRId);
        }
     }
}