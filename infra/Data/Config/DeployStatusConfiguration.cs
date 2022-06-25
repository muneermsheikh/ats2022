using core.Entities;
using core.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class DeployStatusConfiguration : IEntityTypeConfiguration<DeployStatus>
     {
          public void Configure(EntityTypeBuilder<DeployStatus> builder)
          {
               builder.Property(p => p.StatusName).IsRequired().HasMaxLength(50);
               builder.HasIndex(p => p.StatusName).IsUnique();
          }
     }
}