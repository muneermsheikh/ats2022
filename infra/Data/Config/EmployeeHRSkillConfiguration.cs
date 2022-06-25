using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class EmployeeHRSkillConfiguration : IEntityTypeConfiguration<EmployeeHRSkill>
     {
        public void Configure(EntityTypeBuilder<EmployeeHRSkill> builder)
        {
            builder.HasIndex(x => new {x.EmployeeId, x.CategoryId, x.IndustryId}).IsUnique();
            //builder.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId);
        }
     }
}