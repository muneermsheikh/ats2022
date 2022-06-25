using System;

namespace core.ParamsAndDtos
{
    public class OrderItemToReturnDto
    {
        public int SrNo { get; set; }
        public string CategoryName { get; set; }
        public string IndustryName { get; set; }
        public string SourceFrom { get; set; }
        public int Quantity { get; set; }
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public bool RequireAssess { get; set; }=false;
        public DateTime CompleteBefore { get; set; }
        public int Charges { get; set; }
        public int FeeFromClientINR {get; set;}
        public string Status { get; set; }
    }
}