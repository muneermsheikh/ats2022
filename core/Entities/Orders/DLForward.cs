using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Orders
{
    public class DLForward: BaseEntity
    {
        public DLForward()
        {
        }

        public DLForward(int orderItemId, int categoryid, string categoryRefAndName, int customerOfficialId, 
            DateTime dateForwarded, string emailIdForwardedTo, string phoneNoForwardedTo, string whatsAppNoForwardedTo, int loggedInAppUserId)
        {
            OrderItemId = orderItemId;
            CategoryId = categoryid;
            CategoryRefAndName = categoryRefAndName;
            CustomerOfficialId = customerOfficialId;
            DateTimeForwarded = dateForwarded;
            DateOnlyForwarded = dateForwarded.Date;
            EmailIdForwardedTo = emailIdForwardedTo;
            PhoneNoForwardedTo = phoneNoForwardedTo;
            WhatsAppNoForwardedTo = whatsAppNoForwardedTo;
            LoggedInAppUserId = loggedInAppUserId;
        }

        public int OrderItemId { get; set; }
        public int CategoryId {get; set;}
        public string CategoryRefAndName {get; set;}
        public int CustomerOfficialId { get; set; }
        public DateTime DateTimeForwarded { get; set; }=DateTime.Now;
        public DateTime DateOnlyForwarded {get; set;}=DateTime.Now.Date;
        public string EmailIdForwardedTo { get; set; }
        public string PhoneNoForwardedTo { get; set; }
        public string WhatsAppNoForwardedTo { get; set; }
        public int LoggedInAppUserId {get; set;}
    }
}