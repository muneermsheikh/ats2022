using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.ParamsAndDtos;

namespace core.Interfaces
{
     public interface ICVReviewService
     {
          Task<ICollection<EmailMessage>> AddNewCVReview(ICollection<CVReviewSubmitByHRExecDto> cvsSubmittedDto, LoggedInUserDto loggedInUserDto);
          Task<CVRvw> GetCVReview(int candidateid, int orderitemid);
          Task<ICollection<CVRvw>> GetCVReviews (int orderitemid);
          Task<bool> UserIsOwnerOfCVReviewBySupObject(int CVReviewBySupId, int loggedInEmpId);
          Task<bool> DeleteCVSubmittedToHRSupForReview(int CVReviewBySupId);
          Task<ICollection<EmailMessage>> CVReviewByHRSup(LoggedInUserDto loggedInDto, ICollection<CVReviewBySupDto> cvsSubmittedDto);
          Task<ICollection<EmailMessage>> CVReviewByHRM(LoggedInUserDto loggedInDto, ICollection<CVReviewByHRMDto> cvsReviewed);
          Task<bool> DeleteCVSubmittedToHRMForReview(int CVReviewByHRMId);
          Task<ICollection<CVReviewsPendingDto>>PendingCVReviewsByUserIdAsync(int userId);
          Task<ICollection<CVReviewsPendingDto>>PendingCVReviews();
          Task<int> NextReviewBy(int orderItemId);
     }
}