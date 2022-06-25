using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class OrderItemDetailForMsgDto
    {
        public int SrNo{get; set;}
        public int HrSupId { get; set; }
        public int HrmId { get; set; }
        public string CategoryName { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public int ApplicationNo { get; set; }
        public string FullName { get; set; }
        public bool NoReviewBySupervisor { get; set; }
    }
}