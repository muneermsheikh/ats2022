using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class EmployeeQualificationConfiguration : IEntityTypeConfiguration<EmployeeQualification>
     {
        public void Configure(EntityTypeBuilder<EmployeeQualification> builder)
        {
            builder.HasIndex(x => new {x.EmployeeId, x.QualificationId}).IsUnique();
            //builder.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId);
        }
     }
}