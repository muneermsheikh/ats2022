using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class JobDescriptionConfiguration : IEntityTypeConfiguration<JobDescription>
     {
          public void Configure(EntityTypeBuilder<JobDescription> builder)
          {
               //builder.HasIndex(p => p.OrderItemId).IsUnique();
          }
     }
}