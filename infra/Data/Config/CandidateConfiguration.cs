using System;
using core.Entities.Orders;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
     {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {

            builder.HasIndex(x => x.ApplicationNo).IsUnique().HasFilter("[ApplicationNo] > 0");
            
            builder.HasMany(s => s.EntityAddresses).WithOne().OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(s => s.UserPhones).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.UserQualifications).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.UserProfessions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.UserPassports).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.UserAttachments).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.UserExperiences).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property("FirstName").HasColumnType("VARCHAR").HasMaxLength(75).IsRequired().IsUnicode(false);
            builder.Property("SecondName").HasColumnType("VARCHAR").HasMaxLength(75).IsUnicode(false);
            builder.Property("FamilyName").HasColumnType("VARCHAR").HasMaxLength(75).IsUnicode(false);

            
        }
     }
}