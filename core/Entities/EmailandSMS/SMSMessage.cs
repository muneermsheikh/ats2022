using System;

namespace core.Entities.EmailandSMS
{
    public class SMSMessage: BaseEntity
    {
        public SMSMessage()
        {
        }

        public SMSMessage(int userId, DateTime sMSDateTime, string phoneNo, string sMSText, string deliveryResult)
        {
            UserId = userId;
            SMSDateTime = sMSDateTime;
            PhoneNo = phoneNo;
            SMSText = sMSText;
            DeliveryResult = deliveryResult;
        }

        public int UserId { get; set; }
        public DateTime SMSDateTime { get; set; }
        public string PhoneNo { get; set; }
        public string SMSText { get; set; }
        public string DeliveryResult { get; set; }
    }
}