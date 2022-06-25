using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;

namespace core.Interfaces
{
    public interface IAssessmentStandardQService
    {
        Task<ICollection<AssessmentStandardQ>> GetStandardAssessmentQs();
        Task<AssessmentStandardQ> GetStandardAssessmentQ(int id);
        Task<bool> AddStandardAssessmentQ(ICollection<AssessmentStandardQ> Qs);
        Task<bool> EditStandardAssessmentQ(ICollection<AssessmentStandardQ> Qs);
        Task<bool> DeleteStandardAssessmentQ(int id);
        Task<AssessmentStandardQ> CreateStandardAssessmentQ(AssessmentStandardQ stddQ);
        
    }
}