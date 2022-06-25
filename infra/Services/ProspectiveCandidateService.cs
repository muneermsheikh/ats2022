using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class ProspectiveCandidateService : IProspectiveCandidateService
     {
          private readonly ATSContext _context;
          private readonly UserManager<AppUser> _userManager;
          private readonly IUserService _userService;
          private readonly ITokenService _tokenService;

          public ProspectiveCandidateService(ATSContext context, UserManager<AppUser> userManager, IUserService userService, ITokenService tokenService)
          {
               _tokenService = tokenService;
               _userService = userService;
               _userManager = userManager;
               _context = context;
          }

          public async Task<bool> ConvertProspectToCandidateFromProspectiveId(int id)
          {

               var profile = await(from p in _context.ProspectiveCandidates.Where(x => x.Id == id)
                    join i in _context.OrderItems on p.OrderItemId equals i.Id 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new {p, i.CategoryId, cat.Name}
                    ).FirstOrDefaultAsync();

               DateTime dob = new DateTime();
               var age = profile.p.Age.Substring(0,2);

               if (!string.IsNullOrEmpty(age)) {
                    dob = DateTime.Today.AddYears(-Convert.ToInt32(age));
               }

               var profs = new List<UserProfession>();
               profs.Add(new UserProfession{CategoryId=profile.CategoryId});
               var phs = new List<UserPhone>();
               phs.Add(new UserPhone{MobileNo=profile.p.PhoneNo,IsMain=true});
               if(!string.IsNullOrEmpty(profile.p.AlternatePhoneNo)) phs.Add(new UserPhone{MobileNo=profile.p.AlternatePhoneNo});

               return false;
          }

          private async Task<UserProfession> GetCategoryIdAndNameFromCategoryRef(string categoryref)
          {
               if(string.IsNullOrEmpty(categoryref)) return null;
               
               int i = categoryref.IndexOf("-");
               if (i== -1) return null;
               var orderno = categoryref.Substring(0,i);
               var srno = categoryref.Substring(i+1);
               if (string.IsNullOrEmpty(orderno)) return null;
               if (string.IsNullOrEmpty(srno)) return null;
               int iorderno = Convert.ToInt32(orderno);
               int isrno = Convert.ToInt32(srno);

               var qry = await (from o in _context.Orders where o.OrderNo == iorderno 
                    join item in _context.OrderItems on o.Id equals item.OrderId 
                    join c in _context.Categories on item.CategoryId equals c.Id
                    select new {item.CategoryId, c.Name}).FirstOrDefaultAsync();
              
               if (qry == null) return null;

               return new UserProfession{CategoryId=qry.CategoryId, Profession=qry.Name };

          }

          private int GetAgencyIdFromAgencyName (string agencyname)
          {
               var id = _context.Customers
                    .Where(x => x.CustomerName.ToLower() == agencyname.ToLower())
                    .Select(x => x.Id)
                    .FirstOrDefault();
               return id;
          }


          
          public async Task<UserDto> ConvertProspectiveToCandidate(ProspectiveCandidateAddDto dto) 
          {
               //check unique values of PP and Aadhar
               var user = await _userManager.FindByEmailAsync(dto.Email);
               if(user == null) {            //possible if AppUser created, but failed to create Candidate object;
                              //**TODO** why note delete AppUser when Candidate failed to create??
               //Create AppUser before creating the candidate object, bvz canddidate contains AppuserId field
                    user = new AppUser
                    {
                         UserType = "Candidate",
                         DisplayName = dto.KnownAs,
                         KnownAs = dto.KnownAs,
                         Gender = dto.Gender,
                         PhoneNumber = dto.PhoneNo,
                         Email = dto.Email,
                         UserName = dto.Email
                    };
                    var result = await _userManager.CreateAsync(user, dto.Password);
                    if (!result.Succeeded) return null;
               }

               //create roles
               //var succeeded = await _roleManager.CreateAsync(new AppRole{Name="Candidate"});
               var roleResult = await _userManager.AddToRoleAsync(user, "Candidate");
               //if (!roleResult.Succeeded) return null;
                    
               //var userAdded = await _userManager.FindByEmailAsync(registerDto.Email);
               //no need to retreive obj from DB - the object 'user' can be used for the same
               
               var cvDto = new RegisterDto{
                    UserType="Candidate", Gender="M", FirstName = dto.CandidateName, KnownAs = dto.KnownAs, 
                    DisplayName = dto.KnownAs, UserName = dto.Email,
                    Email = dto.Email,UserRole="Candidate", AppUserId = user.Id};


               if (!string.IsNullOrEmpty(dto.Age)) {
                    var age = dto.Age.Substring(0,2);
                    cvDto.DOB = DateTime.Today.AddYears(-Convert.ToInt32(age));
               }
               
               //EntityAddresses
               if (!string.IsNullOrEmpty(dto.CurrentLocation)) {
                    var adds = new List<EntityAddress>();
                    adds.Add(new EntityAddress{City=dto.CurrentLocation});
                    cvDto.EntityAddresses = adds;
               }
               
               //ReferredBy
               int agencyid=0;
               switch (dto.Source.ToLower()) {
                    case "timesjob":
                    case "timesjobs":
                    case "timesjob.com":
                    case "timesjobs.com":
                         agencyid=9;
                         break;
                    case "naukri":
                    case "naukri.com":
                         agencyid=12;
                         break;
                    default:
                         break;
               }
               cvDto.ReferredBy=agencyid;

               
               // finally, create the object candidate
               var cand = await _userService.CreateCandidateObject(cvDto, dto.LoggedInUserId);

               if (cand == null) return null;
               
               _context.Entry(cand).State = EntityState.Added;

               //once succeeded, delete the record from prospective list.

               var prospect = await _context.ProspectiveCandidates.FindAsync(dto.ProspectiveId);
               if(prospect == null) return null;

               _context.Entry(prospect).State=EntityState.Deleted;

               var recordsAffected = await _context.SaveChangesAsync();         //should be 2 - one added, and one deleted

               //UserProfessions
               var prf = await GetCategoryIdAndNameFromCategoryRef(dto.CategoryRef);
               if (prf != null) {
                    var prof = new UserProfession{CategoryId=prf.CategoryId, CandidateId=cand.Id, Profession=prf.Profession};
                    _context.Entry(prof).State = EntityState.Added;
               }

               //UserPhones
               var ph = new UserPhone{MobileNo=dto.PhoneNo,IsMain=true, CandidateId=cand.Id};
               _context.Entry(ph).State=EntityState.Added;
               if(!string.IsNullOrEmpty(dto.AlternatePhoneNo)) {
                    ph =new UserPhone{MobileNo=dto.AlternatePhoneNo, CandidateId=cand.Id};
                    _context.Entry(ph).State=EntityState.Added;
               }

               await _context.SaveChangesAsync();

               //return UserDto object
               var userDtoToReturn = new core.ParamsAndDtos.UserDto
               {
                    DisplayName = user.DisplayName,
                    Token = await _tokenService.CreateToken(user),
                    Email = user.Email
               };

               return userDtoToReturn;
          }



          public async Task<Pagination<ProspectiveCandidate>> GetProspectiveCandidates(ProspectiveCandidateParams pParams)
          {
               /*
               var prospects =await _context.ProspectiveCandidates.Select(x => new {x.Id, x.Status}).ToListAsync();
               foreach(var id in prospects )
               {
                    var histid = await _context.UserHistories.Where(x => x.PersonId == id.Id && x.PersonType == "prospective").Select(x => x.Id).FirstOrDefaultAsync();
                    var status = await _context.UserHistoryItems.Where(x => x.UserHistoryId == histid)
                         .OrderByDescending(x => x.DateOfContact).Select(x => x.ContactResultName).FirstOrDefaultAsync();
                    if(!string.IsNullOrEmpty(status)) {
                         var prosp = await _context.ProspectiveCandidates.FindAsync(id.Id);
                         if(prosp != null) {
                              prosp.Status = status;
                              _context.Entry(prosp).State = EntityState.Modified;
                         }
                    }
               }
               var recordsAffected = await _context.SaveChangesAsync();
               */

               var qry = _context.ProspectiveCandidates.AsQueryable();
               if(pParams.CategoryRef != null) qry = qry.Where(x => x.CategoryRef == pParams.CategoryRef);
               if(pParams.DateAdded.Year > 2000) qry = qry.Where(x => x.Date.Date == pParams.DateAdded.Date);
               if(pParams.Status=="concluded") {
                    qry = qry.Where(x =>  x.Status.Contains("concluded"));
               } else if (pParams.Status == "pending") {
                    qry = qry.Where(x =>  !x.Status.ToLower().Contains("concluded") || x.Status==null);
               } else if (!string.IsNullOrEmpty(pParams.Status)) {
                    qry = qry.Where(x => x.Status == pParams.Status);
               }
               if(pParams.Sort=="name") {qry = qry.OrderBy(x => x.CandidateName);} 
               else if (pParams.Sort=="categoryref") {qry = qry.OrderBy(x => x.CategoryRef);} 
               else if (pParams.Sort == "status") {qry = qry.OrderBy(x => x.Status);}
               else {qry = qry.OrderBy(x => x.Source);}

               var totalCount = await qry.CountAsync();
               var prospectives = await qry.Skip((pParams.PageIndex-1)*pParams.PageSize).Take(pParams.PageSize).ToListAsync();

               return new Pagination<ProspectiveCandidate>(pParams.PageIndex, pParams.PageSize, totalCount, prospectives);
          }

         
          
     }
}