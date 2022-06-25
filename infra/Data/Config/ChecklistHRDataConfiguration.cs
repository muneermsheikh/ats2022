using System;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ChecklistHRDataConfiguration : IEntityTypeConfiguration<ChecklistHRData>
     {
        public void Configure(EntityTypeBuilder<ChecklistHRData> builder)
        {
            builder.HasIndex(x => x.Parameter).IsUnique();
            builder.Property(x => x.Parameter).IsRequired().HasMaxLength(250);
            builder.HasIndex(x => x.SrNo).IsUnique();
            //range defined in data anotations
        }
     }
}