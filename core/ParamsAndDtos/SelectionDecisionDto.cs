using System;

namespace core.ParamsAndDtos
{
    public class SelectionDecisionDto
    {
        public int OrderNo { get; set; }
        public string CustomerName { get; set; }
        public string CategoryName { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public DateTime ReferredOn { get; set; }
        public int SelectionStatusId { get; set; }
        public DateTime SelectionDecisionDate { get; set; }
    }
}