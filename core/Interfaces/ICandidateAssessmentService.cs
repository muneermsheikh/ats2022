using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.HR;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface ICandidateAssessmentService
    {
        Task<CandidateAssessmentWithErrorStringDto> GetorCreateNewCandidateAssessment(bool requireInternalReview, int candidateId, int orderItemId, int loggedInIdentityUserId);
        Task<CandidateAssessment> GetNewAssessmentObject(bool requireInternalReview, int candidateId, int orderItemId, int loggedInIdentityUserId);
        Task<ICollection<CandidateAssessedDto>> GetAssessedCandidatesApproved ();
        Task<MessagesDto> EditCandidateAssessment(CandidateAssessment candidateAssessment, int loggedinEmployeeId);
        Task<bool> DeleteCandidateAssessment(int CandidateAssessmentId);
        Task<bool> DeleteCandidateAssessmentItem(CandidateAssessmentItem assessmentItem);
        Task<CandidateAssessment> GetCandidateAssessment(int candidateId, int orderItemId);   
        Task<CandidateAssessmentAndChecklistDto> GetCandidateAssessmentAndChecklist(int candidateId, int orderItemId, int loggedInEmployeeId, string loggedInUsername);
    }
}