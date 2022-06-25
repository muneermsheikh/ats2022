using System;

namespace core.ParamsAndDtos
{
    public class OrderVeryBriefDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ProjectManagerId { get; set; }
    }
}