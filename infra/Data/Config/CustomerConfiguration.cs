using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
     {
          public void Configure(EntityTypeBuilder<Customer> builder)
          {
               builder.Property(p => p.CustomerType).IsRequired().HasMaxLength(10);
               builder.Property(p => p.CustomerName).IsRequired().HasMaxLength(50);
               builder.Property(p => p.KnownAs).IsRequired().HasMaxLength(25);
               builder.Property(p => p.City).IsRequired().HasMaxLength(25);

               builder.HasIndex(p => new {p.CustomerName, p.City})
                    .IsUnique();

               builder.HasMany(s => s.CustomerIndustries).WithOne().OnDelete(DeleteBehavior.Cascade);
               builder.HasMany(s => s.CustomerOfficials).WithOne().OnDelete(DeleteBehavior.Cascade);
          }
     }
}