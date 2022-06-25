using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CustomerOfficialConfiguration : IEntityTypeConfiguration<CustomerOfficial>
     {
          public void Configure(EntityTypeBuilder<CustomerOfficial> builder)
          {
               builder.Property(p => p.CustomerId).IsRequired();
               builder.Property(p => p.Gender).IsRequired();
               builder.Property(p => p.OfficialName).IsRequired().HasMaxLength(50);
               //builder.HasOne(p => p.Customer).WithMany().HasForeignKey(p => p.CustomerId);
          }
     }
}