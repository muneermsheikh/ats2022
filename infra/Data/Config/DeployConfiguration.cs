using System;

using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class DeployConfiguration : IEntityTypeConfiguration<Deploy>
     {
        public void Configure(EntityTypeBuilder<Deploy> builder)
        {
            builder.HasIndex(x => x.CVRefId);
            builder.Property(x => x.TransactionDate).IsRequired();
            builder.HasOne(p => p.CVRef).WithMany().HasForeignKey(p => p.CVRefId).OnDelete(DeleteBehavior.Restrict);
        }
     }
}