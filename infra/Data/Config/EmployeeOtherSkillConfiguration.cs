using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class EmployeeOtherSkillConfiguration : IEntityTypeConfiguration<EmployeeOtherSkill>
     {
        public void Configure(EntityTypeBuilder<EmployeeOtherSkill> builder)
        {
            builder.HasIndex(x => new {x.EmployeeId, x.SkillDataId}).IsUnique();
            builder.Property(x => x.SkillDataId).IsRequired();
            builder.Property(x => x.SkillLevel).IsRequired();
            //builder.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId);
        }
     }
}