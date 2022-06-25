using core.Entities;
using core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class DateForwardConfiguration : IEntityTypeConfiguration<DLForward>
     {
          public void Configure(EntityTypeBuilder<DLForward> builder)
          {
               builder.Property(p => p.OrderItemId).IsRequired();
               builder.Property(p => p.CustomerOfficialId).IsRequired();
               builder.HasIndex(p => new {p.OrderItemId, p.CustomerOfficialId, p.DateOnlyForwarded}).IsUnique();
               
          }
     }
}