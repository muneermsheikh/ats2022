using System;
using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
     {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(s => s.Department).HasMaxLength(50).IsRequired();
            builder.Property(s => s.Status).HasConversion(
                o => o.ToString(),
                o => (EnumEmployeeStatus) Enum.Parse(typeof(EnumEmployeeStatus), o)
            );

            builder.HasMany(s => s.EmployeeQualifications).WithOne().OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(s => s.HrSkills).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.OtherSkills).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.Email).IsUnique();
            builder.HasIndex(p => p.AadharNo).IsUnique();


           /*
            builder.OwnsOne(o => o.Person , a => 
               { a.WithOwner(); });
            builder.OwnsOne(o => o.Address , a => 
               { a.WithOwner(); });
*/
        }
     }
}