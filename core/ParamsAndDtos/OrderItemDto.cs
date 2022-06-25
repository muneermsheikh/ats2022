using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        public int SrNo { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public int IndustryId { get; set; }
        public string IndustryName { get; set; }
        public string SourceFrom { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public bool RequireAssess { get; set; }=false;
        public DateTime CompleteBefore { get; set; }
        public int? HrExecId { get; set; }
        public int? HrSupId { get; set; }
        public int? HrmId { get; set; }
        public int? AssignedId { get; set; }
        [Required]
        public int Charges { get; set; }
        public int FeeFromClientINR {get; set;}
        public string Status { get; set; }
        public JobDescription JobDescription { get; set; }
        public Remuneration Remuneration { get; set; }
        //public ContractReviewItem ContractReviewItem { get; set; }
    }
}