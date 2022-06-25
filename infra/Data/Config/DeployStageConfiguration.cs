using System;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class DeployStageConfiguration : IEntityTypeConfiguration<DeployStage>
     {
        public void Configure(EntityTypeBuilder<DeployStage> builder)
        {
            builder.HasIndex(x => x.Status).IsUnique();
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Sequence).IsUnique();
            //range defined in data anotations
        }
     }
}