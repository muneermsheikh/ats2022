using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class MessageConfiguration : IEntityTypeConfiguration<EmailMessage>
     {
          public void Configure(EntityTypeBuilder<EmailMessage> builder)
          {
               builder.Property(p => p.SenderUserName).IsRequired().HasMaxLength(50);
               builder.Property(p => p.RecipientId).IsRequired();
               builder.Property(p => p.RecipientUserName).IsRequired().HasMaxLength(50);
               builder.Property(p => p.Content).IsRequired();
               //builder.HasOne(u => u.Recipient).WithMany().OnDelete(DeleteBehavior.Restrict);
               //builder.HasOne(u => u.Recipient).WithMany(m => m.MessagesReceived).OnDelete(DeleteBehavior.Restrict);
               //builder.HasOne(U => U.Sender).WithMany(m => m.MessagesSent).OnDelete(DeleteBehavior.Restrict);
               
              /* builder
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

               builder
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict)
               

               builder.HasOne(x => x.Sender).WithMany().OnDelete(DeleteBehavior.NoAction);
               builder.HasOne(x => x.Recipient).WithMany().OnDelete(DeleteBehavior.NoAction);
               */

          }
     }
}