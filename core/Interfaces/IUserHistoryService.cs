using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.Admin;
using core.Entities.HR;
using core.Entities.Identity;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IUserHistoryService
    {
        Task<UserHistory> AddUserContact(UserHistory userHistory);
        Task<bool> DeleteUserContact(UserHistory userHistory);
        Task<bool> DeleteUserContactById(int userContactId);
        Task<ICollection<ContactResult>> GetContactResults();
        Task<UserHistoryReturnDto> EditContactHistory(UserHistory model, LoggedInUserDto loggedInUserDto);
        Task<bool> EditContactHistoryItems(ICollection<UserHistoryItem> items, int LoggedinEmpId);
        Task<UserHistory> GetHistoryByCandidateId(int candidateid);
        Task<UserHistory> GetOrAddUserHistoryByParams(UserHistoryParams historyParams);
        //Task<UserHistoryDto> GetOrAddUserHistoryByNamePhone(string callerame, string mobileno);
        Task<UserHistory> GetHistoryFromHistoryId(int historyId);
        Task<ICollection<CategoryRefDto>> GetCategoryRefDetails();
    }
}
