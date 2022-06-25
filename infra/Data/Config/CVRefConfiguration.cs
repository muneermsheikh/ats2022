using System;
using core.Entities.HR;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CVRefConfiguration : IEntityTypeConfiguration<CVRef>
     {
          public void Configure(EntityTypeBuilder<CVRef> builder)
          {
               /* builder.Property(s => s.RefStatus).HasConversion(
                o => o.ToString(),
                o => (EnumCVRefStatus) Enum.Parse(typeof(EnumCVRefStatus), o)
               );
               */
               //builder.HasIndex(p => p.OrderItemId);
               builder.HasIndex(p => new{p.CandidateId, p.OrderItemId}).IsUnique();
               builder.HasOne(o => o.OrderItem).WithOne().OnDelete(DeleteBehavior.NoAction);
               builder.HasMany(o => o.Deploys).WithOne().OnDelete(DeleteBehavior.Cascade);
               builder.HasIndex(p => p.OrderItemId).IsUnique(false);
               //builder.HasOne(p => p.OrderItem).WithMany().HasForeignKey(p => p.OrderItemId);
               /*builder.HasMany(o => o.Candidates).WithOne().OnDelete(DeleteBehavior.Restrict);
               builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Restrict);
               */
               //builder.HasOne(o => o.SelectionDecision).WithOne().OnDelete(DeleteBehavior.Cascade);
               
          }
     }
}