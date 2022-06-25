using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.MasterEntities;

namespace core.Interfaces
{
    public interface IAssessmentQBankService
    {
        Task<ICollection<Category>> GetExistingCategoriesInAssessmentQBank();  
        Task<ICollection<AssessmentQBank>> GetAssessmentQBanks();
        Task<AssessmentQBank> GetAssessmentQsOfACategoryByName(string categoryName);
        Task<AssessmentQBank> GetAssessmentQBankByCategoryId(int id);
        Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank model);
        Task<AssessmentQBank> InsertAssessmentQBank(AssessmentQBank model);
    }
}