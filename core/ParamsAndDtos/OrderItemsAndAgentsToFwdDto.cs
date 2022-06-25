using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class OrderItemsAndAgentsToFwdDto
    {
        public ICollection<OrderItemToFwdDto> Items {get; set;}
        public ICollection<AssociateToFwdDto> Agents {get; set;}
        public DateTime DateForwarded {get; set;}
    }
}