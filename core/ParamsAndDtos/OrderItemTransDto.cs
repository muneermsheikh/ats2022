using System;
using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class OrderItemTransDto
    {
        public int OrderItemId { get; set; }
        public string CustomerName { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string CategoryName { get; set; }
        public ICollection<CVReferralDto> CVsReferred {get; set;}
    }

    public class CVReferralDto {
        public int CVRefId {get; set;}
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string PPNo { get; set; }
        public DateTime ReferredOn { get; set; }
        public string ReferralStatus { get; set; }
        public ICollection<CVDeployDto> DeploysDto {get; set;}
    }

    public class CVDeployDto {
        public int CVRefId {get; set;}
        public int CVDeployId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string DeployStatus { get; set; }
        public string NextDeployStatus { get; set; }
        public DateTime nextDeployStatusEstimatedDate { get; set; }
    }
}