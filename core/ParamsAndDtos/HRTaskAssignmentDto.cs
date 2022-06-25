using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class HRTaskAssignmentDto
    {
        public int HRExecutiveId { get; set; }
        public ICollection<int> OrderItemIds { get; set; }
    }
    
}