using System.Collections.Generic;
using System.Threading.Tasks;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IVerifyService
    {
         Task<ICollection<string>> OrderItemIdAndCandidateIdExist(int purpose, ICollection<CandidateAndOrderItemIdDto> canandorderitemids);
        
    }
}