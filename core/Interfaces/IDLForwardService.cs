using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IDLForwardService
    {
        Task<bool> ForwardDLToAgents(OrderItemsAndAgentsToFwdDto itemsAndAgents, int LoggedInUserId);
    }
}