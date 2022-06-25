using System;
using core.Entities.Admin;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserHistoryConfiguration : IEntityTypeConfiguration<UserHistory>
     {
          public void Configure(EntityTypeBuilder<UserHistory> builder)
          {
               builder.Property(x => x.PersonId).IsRequired();
               builder.Property(x => x.PersonName).IsRequired().HasMaxLength(100);
               builder.Property(x => x.PhoneNo).IsRequired().HasMaxLength(15);
               builder.Property(x => x.PersonType).IsRequired().HasMaxLength(15);
               
               builder.Property(x => x.EmailId).HasMaxLength(100);
               
               builder.HasIndex(p => p.PersonType);
               builder.HasIndex(p => p.EmailId).IsUnique();
               builder.HasIndex(p => p.PhoneNo).IsUnique().HasFilter("PhoneNo != ''");
               //builder.HasIndex(x => new{x.PersonId, x.PersonType})
                    //.HasFilter("PersonId > 0") .IsUnique();
               builder.HasMany(o => o.UserHistoryItems).WithOne().OnDelete(DeleteBehavior.Cascade);
          }
     }
}