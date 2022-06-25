using core.Entities;
using core.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class InterviewItemConfiguration : IEntityTypeConfiguration<InterviewItem>
     {
          public void Configure(EntityTypeBuilder<InterviewItem> builder)
          {
               builder.HasIndex(x => x.OrderItemId).IsUnique();
          }
     }
}