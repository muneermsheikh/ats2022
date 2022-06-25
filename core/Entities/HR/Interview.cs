using System;
using System.Collections.Generic;

namespace core.Entities.HR
{
    public class Interview: BaseEntity
    {
        public Interview()
        {
        }

        public Interview(int orderId, int orderNo, DateTime orderDate, int customerId, string customerName, string interviewVenue, 
            DateTime interviewDateFrom, DateTime interviewDateUpto, int interviewLeaderId, string customerRepresentative,
            string interviewMode, ICollection<InterviewItem> interviewitems)
        {
            OrderId = orderId;
            OrderNo = orderNo;
            OrderDate = orderDate;
            CustomerId = customerId;
            CustomerName = customerName;
            InterviewVenue = interviewVenue;
            InterviewDateFrom = interviewDateFrom;
            InterviewDateUpto = interviewDateUpto;
            InterviewLeaderId = interviewLeaderId;
            CustomerRepresentative = customerRepresentative;
            InterviewItems = interviewitems;
            InterviewMode = interviewMode;
        }

        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate {get; set;}
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewVenue { get; set; }
        public DateTime InterviewDateFrom { get; set; }
        public DateTime InterviewDateUpto { get; set; }
        public int InterviewLeaderId { get; set; }
        public string CustomerRepresentative { get; set; }
        public string InterviewStatus {get; set;} = "open";
        public string ConcludingRemarks {get; set;}
        public ICollection<InterviewItem> InterviewItems {get; set;}
    }
}