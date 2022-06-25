using System;
using core.Entities.HR;
using core.Entities.Process;

namespace core.ParamsAndDtos
{
    public class CommonDataDto
    {
        public int ChecklistHRId { get; set; }
        public string CustomerName { get; set; }
        public int ApplicationNo {get; set;}
        public int CandidateId { get; set; }
        public bool Ecnr { get; set; }
        public string CandidateName { get; set; }
        public int OrderNo { get; set; }
        public int OrderId {get; set;}
        public int CategoryId {get; set;}
        public int OrderItemId { get; set; }
        public int OrderItemSrNo { get; set; }
        public int MedicalProcessInchargeEmpId { get; set; }
        public int VisaProcessInchargeEmpId { get; set; }
        public int EmigProcessInchargeId { get; set; }
        public int TravelProcessInchargeId { get; set; }
        public string CategoryName { get; set; }
        public bool RequireInternalReview {get; set;}
        public bool NoReviewBySupervisor { get; set; }
        public int HRExecId {get; set;}
        public int HRSupId { get; set; }
        public int HRExecTaskId {get; set;}
        public int HRMId { get; set; }
        public EnumSelStatus ReviewResultId { get; set; }
        public EnumDeployStatus DeployStageId {get; set;}
        public DateTime DeployStageDate {get; set;}
        public string CandidateDesc {get => "Application No " + ApplicationNo  + "-" + CandidateName + 
            "(Category Ref " + OrderNo + "-" + OrderItemSrNo + " for " + CustomerName + ")";}
    }
}