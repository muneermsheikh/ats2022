using System;
using core.Entities.HR;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
     {
          public void Configure(EntityTypeBuilder<Employment> builder)
          {
               builder.HasIndex(x => x.CVRefId).IsUnique();

               //builder.HasOne(x => x.SelectionDecision).WithOne().OnDelete(DeleteBehavior.Restrict);
          }
     }
}