using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class VerifyService : IVerifyService
     {
          private readonly ATSContext _context;
          private readonly IConfiguration _config;
          private readonly bool _checklistHRMandatory;
          public VerifyService(ATSContext context, IConfiguration config)
          {
               _config = config;
               _context = context;
               _checklistHRMandatory = Convert.ToBoolean(config.GetSection("ChecklistHRMandatory").Value);
          }

          public async Task<ICollection<string>> OrderItemIdAndCandidateIdExist(int purpose, ICollection<CandidateAndOrderItemIdDto> ids)
          {
               var errors = new List<string>();

               bool Exist = false;
               switch (purpose)
               {
                    case 1: //CV REVIEW internally
                         var candidateids = await _context.Candidates
                              .Where(x => ids.Select(x => x.CandidateId).Contains(x.Id))
                              .Select(x => x.Id).ToListAsync();
                         if (candidateids == null) {
                              errors.Add("invalid candidate ids encountered");
                              return errors;
                         } 
                         
                         var orderitemids = await _context.OrderItems
                              .Where(x => ids.Select(x => x.OrderItemId).Contains(x.Id))
                              .Select(x => x.Id)
                              .ToListAsync();
                         if (orderitemids == null) {
                              errors.Add("Invalid orderitem id");
                              return errors;
                         }

                         var qry = await (from i in _context.OrderItems
                                          where  orderitemids.Contains(i.Id)
                                          join o in _context.Orders on i.OrderId equals o.Id
                                          join c in _context.Customers on o.CustomerId equals c.Id
                                          select c.Id).FirstOrDefaultAsync();
                         Exist = qry == 0 ? false : true;

                         if (!Exist) break;

                         //if checklist mandatory, verify all checklistHRItems are accepted.
                         if (!_checklistHRMandatory) break;
                         foreach(var id in ids)
                         {
                              var checklisthr = await _context.ChecklistHRs
                                   .Where(x => x.CandidateId == id.CandidateId && x.OrderItemId == id.OrderItemId)
                                   .Include(x => x.ChecklistHRItems)
                                   .Select(x => new {x.Id, x.ChecklistHRItems})
                                   .FirstOrDefaultAsync();
                              if (checklisthr == null) errors.Add("No checklist record present for candidateId " + 
                                   id.CandidateId + " and orderitemid " + id.OrderItemId);
                              var allAccepted = checklisthr.ChecklistHRItems.Where(x => !x.Accepts).Select(x => x.Accepts).ToList();
                              if (allAccepted.Count > 0) errors.Add("not all checklist items are accepted for " +
                                   "candidateId " + id.CandidateId + " and orderitemid " + id.OrderItemId);
                         }
                         break;
                    default:
                         break;
               }

               return errors;
          }

          public class CandidateIdAndOrderItemId
          {
          }
     }
}