using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Http;

namespace core.Interfaces
{
     public interface IUserService
    {
        //Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, ICollection<IFormFile> UserFormFiles, int loggedInEmployeeId);
        Task<Candidate> CreateCandidateObject(RegisterDto registerDto, int loggedInEmployeeId);
        Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, int loggedInEmployeeId);
        Task<string> CheckIfObjectExists(RegisterDto registerDto);

        Task<Employee> CreateEmployeeAsync(RegisterEmployeeDto registerDto);
        Task<CustomerOfficial> CreateCustomerOfficialAsync(RegisterDto registerDto);
        Task<Pagination<CandidateBriefDto>> GetCandidates(CandidateSpecParams candidateParams);
        Task<Candidate> GetCandidateByIdWithAllIncludes(int id);
        Task<CandidateBriefDto> GetCandidateByAppNo(int appno);
        Task<CandidateBriefDto> GetCandidateBriefById(int candiadteid);
        Task<ICollection<Candidate>> GetCandidatesWithProfessions(CandidateSpecParams param);
        Task<string> GetCategoryNameFromCategoryId(int id);
        Task<string> GetCustomerNameFromCustomerId(int id);
        //Task<Candidate> GetCandidateByIdAsync(int id);
        Task<Candidate> UpdateCandidateAsync(Candidate candidate);
        //Task<Candidate> GetCandidateBySpecsUserIdAsync(int userId);
        //Task<Candidate> GetCandidateBySpecsIdentityIdAsync(int identityUserId);
        Task<ICollection<UserProfession>> EditUserProfessions(UserAndProfessions userProfessions);
        Task<ICollection<CandidateCity>> GetCandidateCityNames();
        Task<string> CheckPPNumberExists(string ppNumber);
        Task<bool> CheckAadharNoExists(string aadharNo);
    }
}