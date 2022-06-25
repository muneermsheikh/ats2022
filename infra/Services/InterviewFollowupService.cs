using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;

namespace infra.Services
{
     public class InterviewFollowupService : IInterviewFollowupService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          public InterviewFollowupService(IUnitOfWork unitOfWork, ATSContext context)
          {
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<ICollection<InterviewItemCandidateFollowup>> AddToInterviewItemCandidatesFollowup(InterviewCandidateFollowupToAddDto followups)
          {
               var fupAdds = new List<InterviewItemCandidateFollowup>();
               if (await _context.InterviewAttendancesStatus.FindAsync(followups.AttendanceStatusId) == null) 
                    throw new System.Exception("Attendance Id " + followups.AttendanceStatusId + " does not exist");
               if (await _context.Employees.FindAsync(followups.ContactedById) == null) 
                    throw new System.Exception("Employee Id " + followups.ContactedById + " does not exist");

               foreach(var fup in followups.followupItems)
               {
                    //check if fup.itnerviewitemcandidateid is valid
                    if (await _context.InterviewItemCandidates.FindAsync(fup.InterviewItemCandidateId) == null) 
                         throw new System.Exception("Record Id " + fup.InterviewItemCandidateId + " does not exist") ;

                    var followup = new InterviewItemCandidateFollowup(fup.InterviewItemCandidateId, 
                         followups.ContactedOn, followups.ContactedById, fup.MobileNoCalled, followups.AttendanceStatusId);
                    
                    switch (followups.AttendanceStatusId)
                    {
                         case 6:
                         case 1003:
                         case 1009:
                         case 1010:
                         case 1011:
                              followup.FollowupConcluded = true;
                              var interviewItemCandidate = await _unitOfWork.Repository<InterviewItemCandidate>().GetByIdAsync(fup.InterviewItemCandidateId);
                              interviewItemCandidate.SelectionStatusId = followups.AttendanceStatusId;
                              _unitOfWork.Repository<InterviewItemCandidate>().Update(interviewItemCandidate);
                              break;
                         default:
                         break;
                    }
                    _unitOfWork.Repository<InterviewItemCandidateFollowup>().Add(followup);
                    fupAdds.Add(followup);
               }
               
               if (fupAdds.Count ==0) return null;
               if (await _unitOfWork.Complete() > 0) return fupAdds;

               return null;
          
          }

          public async Task<ICollection<InterviewItemCandidateFollowup>> EditInterviewItemCandidatesFollowup(ICollection<InterviewItemCandidateFollowup> followups)
          {
              var fupEdits = new List<InterviewItemCandidateFollowup>();
               foreach (var fup in followups)
               {
                    var followupEdit = await _unitOfWork.Repository<InterviewItemCandidateFollowup>().GetByIdAsync(fup.Id);
                    if (followupEdit == null) continue;
                    var followup = new InterviewItemCandidateFollowup(fup.InterviewItemCandidateId, fup.ContactedOn, fup.ContactedById, fup.MobileNoCalled, fup.AttendanceStatusId);
                    
                    switch (fup.AttendanceStatusId)
                    {
                         case 6:
                         case 1003:
                         case 1009:
                         case 1010:
                         case 1011:
                                followup.FollowupConcluded = true;
                              break;
                         default:
                            break;
                    }
                    fupEdits.Add(followupEdit);
                    _context.Entry(followupEdit).CurrentValues.SetValues(fup);
                    _context.Entry(followupEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    //_unitOfWork.Repository<InterviewItemCandidateFollowup>().Update(followup);
               }

               if (fupEdits.Count ==0) return null;
               if (await _unitOfWork.Complete() > 0) return fupEdits;

               return null;
          }

          public async Task<bool> DeleteInterviewItemCandidatesFollowup(ICollection<InterviewItemCandidateFollowup> followups)
          {
               foreach (var followup in followups)
               {
                    _unitOfWork.Repository<InterviewItemCandidateFollowup>().Delete(followup);
               }

               return await _unitOfWork.Complete() > 0;
          }

     }
}