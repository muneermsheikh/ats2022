using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class AssessmentStandardQService : IAssessmentStandardQService
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          public AssessmentStandardQService(ATSContext context, IUnitOfWork unitOfWork)
          {
               _unitOfWork = unitOfWork;
               _context = context;
          }

          public async Task<bool> AddStandardAssessmentQ(ICollection<AssessmentStandardQ> Qs)
          {
                var nextQuestionNo = await _context.AssessmentStandardQs.MaxAsync(x => (int?)x.QNo) ?? 1;
                int ct = 0;
                foreach (var q in Qs)
                {
                    q.QNo = ++nextQuestionNo;
                    _unitOfWork.Repository<AssessmentStandardQ>().Add(q);
                    ct++;
                }
                if (ct == 0) return false;
                return await _unitOfWork.Complete() > 0;

          }

          public async Task<AssessmentStandardQ> CreateStandardAssessmentQ(AssessmentStandardQ stddQ)
          {
               _unitOfWork.Repository<AssessmentStandardQ>().Add(stddQ);
               if (await _unitOfWork.Complete() > 0 ) return stddQ;

               return null;
          }

          public async Task<bool> DeleteStandardAssessmentQ(int id)
          {
               var q = await _context.AssessmentStandardQs.FindAsync(id);
               if (q==null) throw new Exception("invalid record id");

               _unitOfWork.Repository<AssessmentStandardQ>().Delete(q);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditStandardAssessmentQ(ICollection<AssessmentStandardQ> qs)
          {
                foreach (var q in qs)
                {
                    _unitOfWork.Repository<AssessmentStandardQ>().Update(q);
                }
                return await _unitOfWork.Complete() > 0;
          }

          public async Task<AssessmentStandardQ> GetStandardAssessmentQ(int id)
          {
               return await _context.AssessmentStandardQs.FindAsync(id);
          }

          public async Task<ICollection<AssessmentStandardQ>> GetStandardAssessmentQs()
          {
               return await _context.AssessmentStandardQs.OrderBy(x => x.QNo).ToListAsync();
          }
     }
}