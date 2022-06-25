using System;
using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class RemunerationDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public ICollection<RemunerationItemDto> RemunerationItems {get; set;}
    }
}