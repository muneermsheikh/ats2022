using System;
using System.ComponentModel.DataAnnotations;
using core.Entities.Tasks;

namespace core.Entities.EmailandSMS
{
     public class EmailMessage: BaseEntity
    {
          public EmailMessage()
          {
          }

          public EmailMessage(string messageGroup, int senderId, int recipientId, string senderEmailAddress, 
            string senderUserName, string recipientUserName, string recipientEmailAddress, string ccEmailAddress, 
            string bccEmailAddress, string subject, string content, int messageTypeId, int postaction)
          {
               MessageGroup = messageGroup;
               SenderId = senderId;
               RecipientId = recipientId;
               SenderEmailAddress = senderEmailAddress;
               SenderUserName = senderUserName;
               RecipientUserName = recipientUserName;
               RecipientEmailAddress = recipientEmailAddress;
               CcEmailAddress = ccEmailAddress;
               BccEmailAddress = bccEmailAddress;
               Subject = subject;
               Content = content;
               MessageTypeId = messageTypeId;
               PostAction = postaction;
          }

        public string MessageGroup {get; set;}
        [EmailAddress]
        public string SenderEmailAddress{get; set;}
        public string SenderUserName { get; set; }
        public string RecipientUserName { get; set; }  
        [Required, EmailAddress]  
        public string RecipientEmailAddress { get; set; }  
        //[EmailAddress]  
        public string CcEmailAddress { get; set; }  
        //[EmailAddress]  
        public string BccEmailAddress { get; set; }  
        [Required]  
        public string Subject { get; set; }  
        [Required]  
        public string Content { get; set; }  
        public int MessageTypeId {get; set;}    //??
        public DateTime? DateReadOn { get; set; }
        public DateTime? MessageSentOn {get; set;}
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        public int SenderId { get; set; }
        //[ForeignKey("SenderId")]
        //public AppUser Sender { get; set; }
        public int PostAction {get; set;} = (int)EnumPostTaskAction.OnlyComposeEmailMessage;
        public int RecipientId { get; set; }
        //[ForeignKey("RecipientId")]
        //public AppUser Recipient { get; set; }

        //public ICollection<EmailMessage> MessagesReceived {get; set;}
        //public ICollection<EmailMessage> MessagesSent {get; set;}
        
    }
}