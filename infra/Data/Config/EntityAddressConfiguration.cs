using System;
using core.Entities.Orders;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class EntityAddressConfiguration : IEntityTypeConfiguration<EntityAddress>
     {
        public void Configure(EntityTypeBuilder<EntityAddress> builder)
        {
            builder.Property(x => x.Add).IsRequired().HasMaxLength(250).IsUnicode(false);
            //builder.HasOne(p => p.Candidate).WithMany().HasForeignKey(p => p.CandidateId);
        }
     }
}