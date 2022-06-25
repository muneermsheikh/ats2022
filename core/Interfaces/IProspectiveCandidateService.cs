using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IProspectiveCandidateService
    {
        Task<Pagination<ProspectiveCandidate>> GetProspectiveCandidates(ProspectiveCandidateParams pParams);
        Task<UserDto> ConvertProspectiveToCandidate(ProspectiveCandidateAddDto dto);
    }
}