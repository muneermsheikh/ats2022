using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.Admin;
using core.Entities.HR;

namespace core.Entities.Orders
{
    public class OrderItem: BaseEntity
    {
          public OrderItem()
          {
          }
          public OrderItem(int srNo, int orderNo, int categoryId, string categoryName, int industryId, string industryName, 
            string sourceFrom, int quantity, int minCVs, int maxCVs, bool ecnr, bool requireAssess, DateTime completeBefore, 
            int charges, JobDescription jd, Remuneration remun)
          {
               SrNo = srNo;
               OrderNo = orderNo;
               CategoryId = categoryId;
               CategoryName = categoryName;
               IndustryId = industryId;
               IndustryName = industryName;
               SourceFrom = sourceFrom;
               Quantity = quantity;
               MinCVs = minCVs;
               MaxCVs = maxCVs;
               Ecnr = ecnr;
               RequireAssess = requireAssess;
               CompleteBefore = completeBefore;
               Charges = charges;
               JobDescription = jd;
               Remuneration = remun;
          }

          public OrderItem(int orderId, int srNo, int categoryId, string categoryName, int industryId, string industryName, 
            string sourceFrom, int quantity, int minCVs, int maxCVs, bool ecnr, bool requireAssess, DateTime completeBefore, 
            int charges, bool noReviewBySupervisor)
          {
               OrderId = orderId;
               SrNo = srNo;
               CategoryId = categoryId;
               CategoryName = categoryName;
               IndustryId = industryId;
               IndustryName = industryName;
               SourceFrom = sourceFrom;
               Quantity = quantity;
               MinCVs = minCVs;
               MaxCVs = maxCVs;
               Ecnr = ecnr;
               RequireAssess = requireAssess;
               CompleteBefore = completeBefore;
               Charges = charges;
               NoReviewBySupervisor = noReviewBySupervisor;
          }

        public int OrderId { get; set; }
        public int OrderNo {get; set;}
        public int SrNo { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int IndustryId { get; set; }
        public string IndustryName { get; set; }
        public string SourceFrom { get; set; }
        public int Quantity { get; set; }
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public bool IsProcessingOnly { get; set; }=false;
        public bool RequireInternalReview {get; set;}=false;
        public bool RequireAssess { get; set; }=false;
        public DateTime CompleteBefore { get; set; }
        public int? HrExecId { get; set; }
        public string HRExecName { get; set; }
        public bool NoReviewBySupervisor { get; set; }=false;
        public int? HrSupId { get; set; }
        public string HrSupName { get; set; }
        public int? HrmId { get; set; }
        public string HrmName { get; set; }
        public int? AssignedId { get; set; }
        public string AssignedToName { get; set; }
        public int Charges { get; set; }
        public int FeeFromClientINR {get; set;}
        public int Status { get; set; }=(int)EnumOrderItemStatus.NotStarted;
        public bool Checked {get; set;}
        public EnumReviewItemStatus ReviewItemStatusId { get; set; }=EnumReviewItemStatus.NotReviewed;
        public virtual Employee Assigned { get; set; }
        public Category Category { get; set; }
        public int? JobDescriptionId {get; set;}
        [ForeignKey("JobDescriptionId")]
        public virtual JobDescription JobDescription { get; set; }
        public int? RemunerationId {get; set;}
        [ForeignKey("RemunerationId")]
        public virtual Remuneration Remuneration { get; set; }
        public ContractReviewItem ContractReviewItem { get; set; }
        //public Order Order { get; set; }
        public ICollection<CVRef> CVRefs { get; set; }

    }
}