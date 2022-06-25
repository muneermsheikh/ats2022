using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Entities.Process;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IDeployService
    {
        Task<Pagination<CVRefAndDeployDto>> GetPendingDeployments(DeployParams depParams);
        Task<int> CountOfPendingDeployments();
        Task<CVRef> GetDeploymentsById(int cvrefid);
        Task<CVReferredDto> GetDeploymentDto(int cvrefid);
        Task<CVRef> GetDeploymentsByCandidateAndOrderItem(int candidateId, int orderItemId);
        Task<DeployAddedDto> AddDeploymentTransaction(int cvrefId, int loggedInEmployeeId, EnumDeployStatus stageId, DateTime? transDate);
        Task<ICollection<DeployAddedDto>> AddDeploymentTransactions(ICollection<DeployPostDto> deployPostsDto, int loggedInEmployeeId);
        Task<bool> EditDeploymentTransaction (Deploy deploy);
        Task<bool> DeleteDeploymentTransactions (Deploy deploy);
        Task<ICollection<DeployStatusDto>> GetDeployStatuses ();
        
    }
}