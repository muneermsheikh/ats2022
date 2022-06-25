using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.MasterEntities;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IMastersService
    {

    //industry
        Task<Industry> AddIndustry(string industryName);
        Task<bool> EditIndustryAsync(Industry industry);
        Task<bool> DeleteIndustryyAsync(Industry industry);
        Task<Industry> GetIndustry(int id);
        Task<Pagination<Industry>> GetIndustryListAsync(IndustrySpecParams industryParams);
        Task<ICollection<Industry>> GetIndustryListWOPaginationAsync();
        Task<ICollection<Industry>> GetIndustryListLike(string industryname);
        //Task<IReadOnlyList<Industry>> GetIndustryListOfACategoryAsync(IndustryParams industryParams);
        
    //Qualifications
        Task<Qualification> AddQualification(string qualificationName);
        Task<bool> EditQualificationAsync(Qualification qualification);
        Task<bool> DeleteQualificationAsync(Qualification qualification);
        Task<Pagination<Qualification>> GetQualificationListAsync(QualificationSpecParams specParams);
        Task<ICollection<Qualification>> GetListAsync();

    //ReviewItemStatus.Description for Contract REVIEW RESULTS
        Task<ReviewItemData> AddReviewItemData(string reviewDescriptionName, bool isBoolean);
        Task<bool> EditReviewItemDataAsync(ReviewItemData reviewItemData);
        Task<bool> DeleteReviewItemDataAsync(ReviewItemData reviewItemData);
        Task<IReadOnlyList<ReviewItemData>> GetReviewItemDataDescriptionListAsync();

 //ReviewItemStatus.Description for Contract REVIEW RESULTS
        Task<ReviewItemStatus> AddReviewItemStatus(string reviewDescriptionName);
        Task<bool> EditReviewItemStatusAsync(ReviewItemStatus ReviewItemStatus);
        Task<bool> DeleteReviewItemStatusAsync(ReviewItemStatus ReviewItemStatus);
        Task<IReadOnlyList<ReviewItemStatus>> GetReviewItemStatusListAsync();

    //ReviewItemData.Description for Contract REVIEW RESULTS
        Task<SkillData> AddSkillData(string skillname);
        Task<bool> EditSkillDataAsync(SkillData skillData);
        Task<bool> DeleteSkillDataAsync(SkillData skillData);
        Task<IReadOnlyList<SkillData>> GetSkillDataListAsync();

    }
}