using System;
using core.Entities.HR;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CVRvwConfiguration : IEntityTypeConfiguration<CVRvw>
     {
          public void Configure(EntityTypeBuilder<CVRvw> builder)
          {
               builder.HasIndex(p => new{p.CandidateId, p.OrderItemId}).IsUnique();
          }
     }
}