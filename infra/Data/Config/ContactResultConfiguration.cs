using core.Entities;
using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ContactResultConfiguration : IEntityTypeConfiguration<ContactResult>
     {
          public void Configure(EntityTypeBuilder<ContactResult> builder)
          {
               builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
               builder.HasIndex(p => new {p.Name, p.PersonType}).IsUnique();
          }
     }
}