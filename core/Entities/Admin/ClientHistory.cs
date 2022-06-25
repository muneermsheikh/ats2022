using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class ClientHistory : BaseEntity
    {
        public ClientHistory()
        {
        }

        public ClientHistory(int clientId, string clientName, string mobileNo, string emailId, DateTime jobDate, string jobDescription,
            string jobStatus, int jobOwnerId, ICollection<ClientHistoryItem> clientHistoryItems)
        {
            ClientId = clientId;
            ClientName = clientName;
            MobileNo = mobileNo;
            EmailId = emailId;
            JobDate = jobDate;
            JobDescription = jobDescription;
            JobStatus = jobStatus;
            JobOwnerId = jobOwnerId;
            ClientHistoryItems = clientHistoryItems;
        }

        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public DateTime JobDate { get; set; }
        public string JobDescription { get; set; }
        public string JobStatus { get; set; }="not started";
        public int JobOwnerId {get; set;}
        public ICollection<ClientHistoryItem> ClientHistoryItems { get; set; }
    }
}