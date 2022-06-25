using core.Entities;
using core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class AppUserRoleConfiguration : IEntityTypeConfiguration<AppUserRole>
     {
          
          public void Configure(EntityTypeBuilder<AppUserRole> builder)
          {
               
               builder.HasIndex(x => new {x.RoleId, x.UserId}).IsUnique();
               
               //modelBuilder.Entity<StudentCourse>().HasKey(sc => new { sc.StudentId, sc.CourseId });
               //builder.HasKey(ur => new {ur.AppUserId, ur.RoleId});
               
               //builder.HasIndex(ur => new { ur.RoleId, ur.AppUserId}).IsUnique();
               //builder.HasKey(ur => new {ur.User.Id, ur.RoleId});
               
          }
     }
}