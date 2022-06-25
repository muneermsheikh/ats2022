using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class JDDto
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CategoryName { get; set; }
        public string JobDescInBrief { get; set; }
        public string QualificationDesired { get; set; }
        public int ExpDesiredMin { get; set; }
        public int ExpDesiredMax { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
    }
}