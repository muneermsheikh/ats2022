using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.HR;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IInterviewFollowupService
    {
        Task<ICollection<InterviewItemCandidateFollowup>> AddToInterviewItemCandidatesFollowup(InterviewCandidateFollowupToAddDto followups);
        Task<ICollection<InterviewItemCandidateFollowup>> EditInterviewItemCandidatesFollowup(ICollection<InterviewItemCandidateFollowup> followups);
        Task<bool> DeleteInterviewItemCandidatesFollowup(ICollection<InterviewItemCandidateFollowup> followups);

    }
}