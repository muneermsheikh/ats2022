using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class RemunerationConfiguration : IEntityTypeConfiguration<Remuneration>
     {
          public void Configure(EntityTypeBuilder<Remuneration> builder)
          {
               //builder.HasIndex(p => p.OrderItemId).IsUnique();
          }
     }
}