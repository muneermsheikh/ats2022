using core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CustomerIndustryConfiguration : IEntityTypeConfiguration<CustomerIndustry>
     {
          public void Configure(EntityTypeBuilder<CustomerIndustry> builder)
          {
               builder.Property(p => p.CustomerId).IsRequired();
               builder.Property(p => p.IndustryId).IsRequired();
               
               builder.HasIndex(p => new {p.CustomerId, p.IndustryId}).IsUnique();
               //builder.HasOne(p => p.Customer).WithMany().HasForeignKey(p => p.CustomerId);
          }
     }
}