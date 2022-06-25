using System;
using core.Entities.EmailandSMS;
using core.Params;

namespace core.Params
{
    public class EmailMessageSpecParams: ParamPages
    {
        public int? Id { get; set; }
        public string Username {get; set;}
        public string Container {get; set;}
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        /*public DateTime? MessageSentFrom {get; set;}
        public DateTime? MessageSentUpto {get; set;}
        public DateTime? MessageRecdFrom {get; set;} 
        public DateTime? MessageRecdUpto {get; set;}
        public string Subject { get; set; }
        */
        public EnumMessageType? MessageTypeId {get; set;}
        //public string ContentsLike {get; set;}

    }
}