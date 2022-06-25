using System;
using core.Entities.HR;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ChecklistHRConfiguration : IEntityTypeConfiguration<ChecklistHR>
     {
          public void Configure(EntityTypeBuilder<ChecklistHR> builder)
          {
               builder.HasIndex(p => p.OrderItemId);
               builder.HasIndex(p => new{p.CandidateId, p.OrderItemId}).IsUnique();
               
               builder.HasMany(s => s.ChecklistHRItems).WithOne().OnDelete(DeleteBehavior.Cascade);
          }
     }
}