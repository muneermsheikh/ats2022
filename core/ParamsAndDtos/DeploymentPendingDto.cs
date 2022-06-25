using System;
using core.Entities.Process;

namespace core.ParamsAndDtos
{
    public class DeploymentPendingDto
    {
        public DeploymentPendingDto()
        {
        }

        public int CVRefId { get; set; }
        public int ApplicationNo {get; set;}
        //public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CustomerName { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        //public int OrderId {get; set;}
        //public int CategoryId {get; set;}
        //public int OrderItemId { get; set; }
        //public int OrderItemSrNo { get; set; }
        public string CategoryName { get; set; }
        public EnumDeployStatus DeployStageId { get; set; }
        public DateTime DeployStageDate { get; set; }
        public EnumDeployStatus NextStageId { get; set; }
        public DateTime NextEstimatedStageDate { get; set; }

        /* public string CandidateDesc {get => "Application No " + ApplicationNo  + "-" + CandidateName + 
            "(Category Ref " + OrderNo + "-" + OrderItemSrNo + " for " + CustomerName + ")";}
        */
    }
}