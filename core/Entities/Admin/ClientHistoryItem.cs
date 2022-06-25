using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class ClientHistoryItem : BaseEntity
    {
        public ClientHistoryItem()
        {
        }

        public ClientHistoryItem(DateTime transDate, int clientOfficialId, string clientOfficialName, string transDescription, 
        string transStatus, int nextStatusResponsibilityId, bool issueReminder, DateTime nextStatusOn, int loggedinUserId)
        {
            TransDate = transDate;
            ClientOfficialId = clientOfficialId;
            ClientOfficialName = clientOfficialName;
            TransDescription = transDescription;
            TransStatus = transStatus;
            NextStatusResponsibilityId = nextStatusResponsibilityId;
            IssueReminder = issueReminder;
            NextStatusOn = nextStatusOn;
            LoggedinUserId = loggedinUserId;
        }

        public int ClientHistoryId { get; set; }
        public DateTime TransDate { get; set; }
        public int ClientOfficialId { get; set; }
        public string ClientOfficialName { get; set; }
        public string TransDescription { get; set; }
        public string TransStatus { get; set; }
        public int NextStatusResponsibilityId { get; set; }
        public bool IssueReminder { get; set; }
        public DateTime NextStatusOn { get; set; }
        public int LoggedinUserId { get; set; }
    }

}