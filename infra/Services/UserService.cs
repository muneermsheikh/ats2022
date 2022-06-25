using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using core.Entities;
using core.Entities.Admin;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace infra.Services
{
     public class UserService : IUserService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly UserManager<AppUser> _userManager;
          private readonly ITaskService _taskService;
          private readonly IComposeMessagesForAdmin _composeMsgAdmin;
          private readonly IComposeMessagesForHR _composeMsgHR;
          private readonly IConfiguration _config;
          private readonly IMapper _mapper;
          //private readonly ILogger _logger;
          //private readonly TokenService _tokenService;
          public UserService(IUnitOfWork unitOfWork, ATSContext context, ITaskService taskService, IConfiguration config,
               UserManager<AppUser> usermanager, IComposeMessagesForAdmin composeMsgAdmin, IComposeMessagesForHR composeMsgHR,
               IMapper mapper   //, ILogger logger
               //, TokenService tokenService
               )
          {
               //_tokenService = tokenService;
               _composeMsgHR = composeMsgHR;
               _userManager = usermanager;
               _context = context;
               _unitOfWork = unitOfWork;
               _taskService = taskService;
               _composeMsgAdmin = composeMsgAdmin;
               _config = config;
               _mapper = mapper;
               //_logger = logger;
          }


          public async Task<Candidate> CreateCandidateObject(RegisterDto registerDto, int loggedInEmployeeId)
          {
               var NextAppNo = await _context.Candidates.MaxAsync(x => x.ApplicationNo);
               NextAppNo = NextAppNo == 0 ? 10001 : NextAppNo+1;
               var firstAdd = registerDto.EntityAddresses?.FirstOrDefault();
               if (registerDto.Address == null) {
                    if (registerDto.EntityAddresses != null && registerDto.EntityAddresses.Count() > 0)
                         registerDto.Address= new Address{
                              Gender = registerDto.Gender,
                              FirstName = registerDto.FirstName, 
                              SecondName = registerDto.SecondName,
                              FamilyName = registerDto.FamilyName,
                              AddressType = firstAdd.AddressType,
                              Add = firstAdd.Add,
                              City = firstAdd.City,
                              StreetAdd = firstAdd.StreetAdd,
                              District = firstAdd.District,
                              DOB = registerDto.DOB,
                              Country = firstAdd.Country
                    }; 
               }

               //registerDto.Address is not forwarded by client, but is populated here from the collection EntityAddresses
               var cand = new Candidate(registerDto.Gender, registerDto.AppUserId, 
                    registerDto.AppUserIdNotEnforced ? registerDto.AppUserIdNotEnforced : true,
                    NextAppNo, registerDto.FirstName, registerDto.SecondName ?? "", registerDto.FamilyName ?? "", registerDto.DisplayName, 
                    registerDto.DOB, registerDto.PlaceOfBirth ??"", registerDto.AadharNo??"", registerDto.Email, registerDto.Introduction ?? "",
                    registerDto.Interests ?? "", registerDto.NotificationDesired, registerDto.Nationality, registerDto.CompanyId == null ? 0 : (int)registerDto.CompanyId, 
                    registerDto.PpNo ?? "", registerDto.EntityAddresses?.Where(x => x.IsMain).Select(x => x.City).FirstOrDefault(),
                    registerDto.EntityAddresses?.Where(x => x.IsMain).Select(x => x.Pin).FirstOrDefault(), 
                    registerDto.ReferredBy,
                    registerDto.UserQualifications, registerDto.UserProfessions,
                    registerDto.UserPassports, null);
               
               cand.Created = DateTime.UtcNow;
               cand.LastActive = DateTime.UtcNow;

               //PP No is unique in the db - include only those passports that do not already exist in the database
               var lstPP = new List<UserPassport>();
               if (cand.UserPassports != null) {
                    foreach (var pp in cand.UserPassports)
                    {
                         var existingPP = await _context.UserPassports.Where(c => c.PassportNo == pp.PassportNo).FirstOrDefaultAsync();
                         if (existingPP == null) lstPP.Add(pp);
                    }
               }
               
               cand.UserPassports = lstPP;

               var lstP = new List<UserPhone>();
               if (registerDto.UserPhones != null )cand.UserPhones = registerDto.UserPhones;
               /*
               if (registerDto.UserPhones != null)
               {
                    foreach (var p in registerDto.UserPhones)
                    {
                         var existingP = await _context.UserPhones.Where(c => c.MobileNo == p.MobileNo).FirstOrDefaultAsync();
                         if (existingP == null)
                         {
                              lstP.Add(p);
                         }
                    }
               }
               cand.UserPhones = lstP;
               */

               if (registerDto.UserProfessions != null) cand.UserProfessions = registerDto.UserProfessions;
               /*{
                    var qry = (from p in registerDto.UserProfessions
                               group p by new {p.CategoryId, p.IndustryId} into g
                               where g.Count() > 1
                               select g.Key);
                    cand.UserProfessions = qry.Count() == 0 && qry != null ? registerDto.UserProfessions : null;
               } 
               */

               if (registerDto.UserQualifications != null) cand.UserQualifications=registerDto.UserQualifications;
          /*
               {
                    var qry = (from p in cand.UserQualifications
                               group p by p.QualificationId into g
                               where g.Count() > 1
                               select g.Key);

                    cand.UserQualifications = qry.Count() == 0 && qry != null ? registerDto.UserQualifications : null;
               }
          */
               
               return cand;
          }

          public async Task<string> CheckIfObjectExists(RegisterDto registerDto)
          {
               string ErrorString="";
               switch (registerDto.UserType.ToLower())
               {
                    case "candidate":
                         var ppno = registerDto.UserPassports.Where(x => x.IsValid).Select(x => x.PassportNo).FirstOrDefault();
                         ErrorString = await PPExists(ppno);
                         break;
                    case "official":
                         var ph = registerDto.UserPhones.Where(x => x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
                         if(string.IsNullOrEmpty(ph)) break;
                         ErrorString = await OfficialExists(registerDto.FirstName + " " + registerDto.FamilyName, ph);
                         break;
                    default:
                         break;
               }
               return ErrorString;
          }

          private async Task<string> OfficialExists(string officialname, string mobileno)
          {
               var customer = await(from o in _context.CustomerOfficials 
                    where o.OfficialName.ToLower() == officialname.ToLower()
                         && o.IsValid
                         && o.Mobile == mobileno
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new {c.CustomerName}
                    ).FirstOrDefaultAsync();
               if(customer==null) return "";
               return customer.CustomerName;
          }
          //returns candidateId holding the passport no.
          private async Task<string> PPExists(string PPNo)
          {
               var candidate = await (from p in _context.UserPassports
                         where p.PassportNo.ToLower() == PPNo.ToLower() && p.IsValid
                    join c in _context.Candidates on p.CandidateId equals c.Id 
                    select new {c.FullName, c.ApplicationNo}
                    ).FirstOrDefaultAsync();
               if (candidate==null) return "";
               return candidate.FullName + ", application no.:" + candidate.ApplicationNo;
          }

          private async Task<string> AadharNoExists(string aadharno)
          {
               var candidate = await _context.Candidates.Where(x => x.AadharNo==aadharno).Select(x => new {x.FullName, x.ApplicationNo}).FirstOrDefaultAsync();
               if (candidate==null) return "";
               return candidate.FullName + ", application no.:" + candidate.ApplicationNo;
          }
          
          public async Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, int loggedInEmployeeId)
          {
               var cand = await CreateCandidateObject(registerDto, loggedInEmployeeId);

               _unitOfWork.Repository<Candidate>().Add(cand);

               if (registerDto.NotificationDesired) {
                    var paramsDto = new CandidateMessageParamDto{Candidate = cand, DirectlySendMessage = false};
                    await _composeMsgHR.ComposeHTMLToAckToCandidateByEmail(paramsDto);
                    await _composeMsgHR.ComposeMsgToAckToCandidateBySMS(paramsDto);
               }
               
               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               var appuser = await _userManager.FindByIdAsync(registerDto.AppUserId.ToString());
               if(appuser != null) {
                    appuser.loggedInEmployeeId=cand.Id;
                    await _userManager.UpdateAsync(appuser);
               } 
               return cand;
          }

     //employees
          public async Task<Employee> CreateEmployeeAsync(RegisterEmployeeDto registerDto)
          {
               /*
               //CHECK IF PHONES ALREADY TAKEN
               var ph = await _context.UserPhones.Where(x => 
                    registerDto.UserPhones.Select(x => x.PhoneNo).ToList().Contains(x.PhoneNo)).ToListAsync();
               if(ph!=null & ph.Count() >0) {
                    return null;
               }
               */

               /* var person = new Person(registerDto.Gender, registerDto.FirstName, registerDto.SecondName,
                    registerDto.FamilyName, registerDto.DisplayName, registerDto.DOB,
                    registerDto.PlaceOfBirth, registerDto.AadharNo, "", registerDto.Nationality);
               */
               var qs = new List<EmployeeQualification>();
               if(registerDto.EmployeeQualifications !=null && registerDto.EmployeeQualifications.Count > 0)
               {
                    foreach(var q in registerDto.EmployeeQualifications)
                    {
                         qs.Add(new EmployeeQualification(q.QualificationId, q.IsMain));
                    }
               }

               var phs = new List<EmployeePhone>();
               if(registerDto.EmployeePhones != null && registerDto.EmployeePhones.Count() > 0) {
                    foreach(var p in registerDto.EmployeePhones)
                    {
                         phs.Add(new EmployeePhone(p.MobileNo, p.IsMain));
                    }
               }
               phs = phs.Count() > 0 ? phs : null;

               var hrskills = new List<EmployeeHRSkill>();
               if(registerDto.HrSkills!=null && registerDto.HrSkills.Count > 0) {
                    foreach(var h in registerDto.HrSkills) {
                         hrskills.Add(new EmployeeHRSkill(h.CategoryId,h.IndustryId, h.SkillLevel));
                    }
               }
               hrskills = hrskills.Count() > 0 ? hrskills : null;

               var otherskills = new List<EmployeeOtherSkill>();
               if(registerDto.OtherSkills != null && registerDto.OtherSkills.Count() > 0) {
                    foreach(var o in registerDto.OtherSkills) {
                         otherskills.Add(new EmployeeOtherSkill(o.SkillDataId, o.SkillLevel, o.IsMain));
                    }
               }
               otherskills = otherskills.Count() > 0 ? otherskills: null;
               
               var employeeAddresses = new List<EmployeeAddress>();
               if (registerDto.EmployeeAddresses!=null && registerDto.EmployeeAddresses.Count > 0) {
                    foreach(var a in registerDto.EmployeeAddresses) {
                         employeeAddresses.Add(new EmployeeAddress(a.AddressType, a.Add, a.StreetAdd,
                         a.City, a.Pin, a.State, a.District, a.Country, a.IsMain));
                    }
               }
               
               var emp = new Employee(registerDto.AppUserId, registerDto.Gender, registerDto.FirstName,
                    registerDto.SecondName, registerDto.FamilyName, registerDto.DisplayName,
                    registerDto.DOB, registerDto.AadharNo, qs, registerDto.DOJ, registerDto.Department, 
                    registerDto.Position, registerDto.Email, hrskills, otherskills, employeeAddresses);

               _unitOfWork.Repository<Employee>().Add(emp);

               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               return emp;
          }

          public async Task<CustomerOfficial> CreateCustomerOfficialAsync(RegisterDto registerDto)
          {
               
               var off = new CustomerOfficial(registerDto.AppUserId, registerDto.Address.Gender, registerDto.Position, 
                    registerDto.Address.FirstName + " " + registerDto.Address.SecondName + " " + registerDto.Address.FamilyName, registerDto.Position, 
                    registerDto.UserPhones.Where(x => !string.IsNullOrEmpty(x.MobileNo)).Select(x => x.MobileNo).FirstOrDefault(),  
                    registerDto.UserPhones.Where(x => x.IsMain && !string.IsNullOrEmpty(x.MobileNo)).Select(x => x.MobileNo).FirstOrDefault(),
                    registerDto.Email, "", true);
                    
               _unitOfWork.Repository<CustomerOfficial>().Add(off);

               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               return off;
          }


          public async Task<AppUser> CreateUserAsync(RegisterDto dto)
          {
               //for customer and vendor official, customer Id is mandatory
               if (dto.UserType.ToLower() =="employee" && string.IsNullOrEmpty(dto.AadharNo)) {
                    throw new Exception("Exception Code 400 - for employees, Aadhar number is mandatory");
                    //return null;
               }

               if (dto.UserType.ToLower()=="official" && (int)dto.CompanyId==0) {
                    throw new Exception ("Error 400 - For officials, customer Id is essential");
                    //return null;
               }
               
               if (dto.UserPhones !=null && dto.UserPhones.Count() > 0) {
                    foreach(var ph in dto.UserPhones) {
                         if (ph.MobileNo == "") return null;     // BadRequest(new ApiResponse(400, "either phone no or mobile no must be mentioned"));
                    }
               }

               var objPP = new UserPassport();

               if(string.IsNullOrEmpty(dto.PpNo)) {
                    objPP = null;
               } else {
                    objPP = new UserPassport(dto.PpNo, dto.Nationality, dto.PPValidity);
               }

               var user = new AppUser
               {
                    UserType = dto.UserType,
                    DisplayName = dto.DisplayName,
                    //Address = dto.Address,
                    //UserPassport = objPP,

                    Email = dto.Email,
                    UserName = dto.Email
               };

               var result = await _userManager.CreateAsync(user, dto.Password);

               if (!result.Succeeded) throw new Exception("Exception Code 400 - bad request - failed to create the identity user");

               var userDtoToReturn = new UserDto
               {
                    DisplayName = user.DisplayName,
                    //Token = _tokenService.CreateToken(user),
                    Email = user.Email
               };

               //var userAdded = await _userManager.FindByEmailAsync(dto.Email);
               return user;
          }

          public async Task<bool> CheckEmailExistsAsync(string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }

          public async Task<bool> CheckAadharNoExists(string aadharNo)
          {
               var emp = await _context.Employees.Where(x => x.AadharNo == aadharNo).FirstOrDefaultAsync();
               return (emp != null);
          }

          public async Task<ICollection<UserProfession>> EditUserProfessions(UserAndProfessions userProfessions)
          {
               var candidateId = userProfessions.CandidateId;

               var selectedProfessions = new List<UserProfession>();
               foreach(var item in userProfessions.CandidateProfessions)
               {
                    if (await _unitOfWork.Repository<Category>().GetByIdAsync(item.CategoryId) != null)  
                         // check if the profession exists in DB before adding to selectedProfessions
                         selectedProfessions.Add(new UserProfession(
                         candidateId, item.CategoryId, item.IndustryId, item.IsMain));
               } 

               if (selectedProfessions.Count() == 0 ) 
                    throw new Exception ("none of the professions of the Candidate exist on record");
                    
               //get the candidate Professions as exist in DB.  Add to the DB those professions that do not
               //existing in this DB set (selectedProfessions.ExceptWhatExist in Db)
               var existingUserProfessions = await _unitOfWork.Repository<UserProfession>()
                    .ListAsync(new UserProfessionsSpecs(candidateId));
                         //
               var selectedCategoryIds = userProfessions.CandidateProfessions.Select(x => x.CategoryId).ToArray();
               var existingUserCategoryIds = existingUserProfessions.Select(x => x.CategoryId).ToList();
               foreach(var p in selectedProfessions.Where(x => !existingUserCategoryIds.Contains(x.CategoryId)).ToList())
               {
                    _unitOfWork.Repository<UserProfession>().Add(new UserProfession(candidateId, p.CategoryId, p.IndustryId, p.IsMain));
               }
               
               //remove professions
               //int ct = 0;
               //var selectedIds = selectedProfessions.Select(x => x.CategoryId).ToList();
               foreach(var p in existingUserProfessions.Where(x => !selectedProfessions.Select(x => x.CategoryId).ToList().Contains(x.CategoryId)).ToList())
               {
                    _unitOfWork.Repository<UserProfession>().Delete(p);
                    //ct++;
               }
               
               if(await _unitOfWork.Complete() == 0) throw new Exception("Failed to edit the user professions, Or the records did not need to change");

               return selectedProfessions;
          }

          public async Task<Pagination<CandidateBriefDto>> GetCandidates(CandidateSpecParams candidateParams)
          {
               var qry = _context.Candidates.AsQueryable();
               if (candidateParams.CandidateId.HasValue) {
                    qry = qry.Where(x => x.Id == candidateParams.CandidateId);
               } else if(candidateParams.AppUserId.HasValue) {
                    qry = qry.Where(x => x.AppUserId == candidateParams.AppUserId);
               } else if (!string.IsNullOrEmpty(candidateParams.Email)) {
                    qry = qry.Where(x => x.Email.ToLower() == candidateParams.Email);
               } else if (!string.IsNullOrEmpty(candidateParams.Mobile)) {
                    qry = qry.Include(x => x.UserPhones.Where(x => x.MobileNo==candidateParams.Mobile));
               } else if (!string.IsNullOrEmpty(candidateParams.PassportNo)) {
                    qry = qry.Include(x => x.UserPassports.Where(x => x.PassportNo==candidateParams.PassportNo));
               } else if (candidateParams.ApplicationNoFrom.HasValue) {
                    if(candidateParams.ApplicationNoUpto.HasValue) {
                         qry = qry.Where(x => x.ApplicationNo >= candidateParams.ApplicationNoFrom && x.ApplicationNo <= candidateParams.ApplicationNoUpto);
                    } else {
                         qry = qry.Where(x => x.ApplicationNo == candidateParams.ApplicationNoFrom);
                    }
               } else {
                    if(candidateParams.RegisteredFrom.HasValue) {
                         if (candidateParams.RegisteredUpto.HasValue) {
                              qry = qry.Where(x => 
                                   DateTime.Compare(x.Created, Convert.ToDateTime(candidateParams.RegisteredFrom)) == 0);
                         } else {
                              qry = qry.Where(x => 
                                   DateTime.Compare(x.Created, Convert.ToDateTime(candidateParams.RegisteredFrom)) >= 0
                                   && DateTime.Compare(x.Created, Convert.ToDateTime(candidateParams.RegisteredUpto)) <=0);
                         }
                    }
                    if(candidateParams.ProfessionId.HasValue) {
                         qry = qry.Include(x => x.UserProfessions.Where(x => x.CategoryId==candidateParams.ProfessionId));
                    }
                    if(candidateParams.AssociateId.HasValue) qry = qry.Where(x => x.ReferredBy==candidateParams.AssociateId);
                    if(!string.IsNullOrEmpty(candidateParams.CandidateStatus)) qry = qry.Where(x => x.CandidateStatus==candidateParams.CandidateStatus);
               }

               var candidates = await qry.ToListAsync();
               int totalItems = candidates == null ? 0 : candidates.Count();

               var dtos = new List<CandidateBriefDto>();
                foreach(var cand in candidates)
               {
                    if(cand.CandidateStatus=="") cand.CandidateStatus="Not Referred";
                    if (cand.UserProfessions != null && cand.UserProfessions.Count > 0) {
                         foreach(var prof in cand.UserProfessions) {
                              if (string.IsNullOrEmpty(prof.Profession)) prof.Profession = await GetCategoryNameFromCategoryId(prof.CategoryId);
                         }
                    }
                    dtos.Add(new CandidateBriefDto{Id = cand.Id, FullName = cand.FirstName + cand.SecondName??"" + " " + cand.FamilyName??"", 
                         City = cand.City, ApplicationNo = cand.ApplicationNo, ReferredById=cand.ReferredBy,
                         ReferredByName= await GetCustomerNameFromCustomerId(cand.ReferredBy),
                         UserProfessions = cand.UserProfessions});
               }
               
               return new Pagination<CandidateBriefDto>(candidateParams.PageIndex, candidateParams.PageSize, totalItems, dtos);

          }

          public async Task<Candidate> GetCandidateByIdWithAllIncludes(int id)
          {
               return await _context.Candidates.Where(x => x.Id == id)
                    .Include(x => x.UserPhones)
                    .Include(x => x.UserQualifications)
                    .Include(x => x.EntityAddresses)
                    .Include(x => x.UserPassports)
                    .Include(x => x .UserAttachments)
                    .Include(x => x.UserExperiences)
                    .Include(x => x.UserProfessions)
               .FirstOrDefaultAsync();
          }
          public async Task<ICollection<Candidate>> GetCandidatesWithProfessions(CandidateSpecParams param)
          {
               var query = _context.Candidates.AsQueryable();

               if (param.ApplicationNoFrom.HasValue && param.ApplicationNoUpto.HasValue)
               {
                    query = query.Where(x => x.ApplicationNo >= param.ApplicationNoFrom && 
                         x.ApplicationNo <= param.ApplicationNoUpto);
               }
               if (param.AssociateId.HasValue)
               {
                    query = query.Where(x => x.CompanyId == param.AssociateId);
               }
               if (param.ProfessionId.HasValue)
               {
                    var candidateIds = await _context.UserProfessions.Where(x => x.CategoryId == param.ProfessionId).Select(x => x.CandidateId).Distinct().ToListAsync();
                    query = query.Where(x => candidateIds.Contains(x.Id));
               }
               if (param.RegisteredFrom.HasValue)  {
                    if (param.RegisteredUpto.HasValue) {
                         query = query.Where(x => 
                              (DateTime.Compare(x.Created, Convert.ToDateTime(param.RegisteredFrom)) <= 0)
                              && (DateTime.Compare(x.Created, Convert.ToDateTime(param.RegisteredUpto)) >=0));
                    } else {
                         query = query.Where(x => 
                              DateTime.Compare(x.Created, Convert.ToDateTime(param.RegisteredFrom)) < 1);
                    }
               }
               
               if (param.IncludeUserProfessions) query = query.Include(x => x.UserProfessions);
               
               //var qry = await query.ProjectTo<CandidateBriefDto>(_mapper.ConfigurationProvider).ToListAsync();
               return await query.ToListAsync();
               //return new Pagination<CandidateBriefDto>(param.PageIndex, param.PageSize, qry.Count(), qry);
               

          }
          public async Task<Candidate> UpdateCandidateAsync(Candidate model )
          {
               var existingObject = await _context.Candidates.Where(x => x.Id == model.Id)
                    .Include(x => x.EntityAddresses)
                    .Include(x => x.UserPhones)
                    .Include(x => x.UserQualifications)
                    .Include(x => x.UserProfessions)
                    .Include(x => x.UserExperiences)
                    //.Include(x => x.UserAttachments)
                    .Include(x => x.UserPassports)
                    .AsNoTracking()
               .FirstOrDefaultAsync();

               if (existingObject == null) return null;
               //update top level entity, i.e. candidate, without related obj

               _context.Entry(existingObject).CurrentValues.SetValues(model);

               //start updating related entities
               //start with deleting records from DB whicha are not present in the model
               
               if(existingObject.EntityAddresses != null) {
                    foreach(var existingItem in existingObject.EntityAddresses.ToList())  {
                         if (!model.EntityAddresses.Any(c => c.Id == existingItem.Id && c.Id != default(int)))  {
                              _context.EntityAddresses.Remove(existingItem);
                              _context.Entry(existingItem).State = EntityState.Deleted;
                         }
                    }
                    
                    foreach(var item in model.EntityAddresses)
                    {
                         var existingItem = existingObject.EntityAddresses.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null)     //record present in DB, therefore update DB record with values from the model
                         {
                              if (item.CandidateId == 0) item.CandidateId = model.Id;
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {       //insert new record
                              var newObj = new EntityAddress(item.AddressType, item.Add, item.StreetAdd,
                                   item.City, item.Pin, item.State, item.District, item.Country, item.IsMain);
                              existingObject.EntityAddresses.Add(newObj);
                              _context.Entry(newObj).State = EntityState.Added;
                         }
                    }
               } 

               //UserPhones
               if (model.UserPhones != null) {
                    if(existingObject.UserPhones != null) {
                         foreach(var existingItem in existingObject.UserPhones.ToList()) {
                              if (!model.UserPhones.Any(c => c.Id == existingItem.Id && c.Id != default(int))) {
                                   _context.UserPhones.Remove(existingItem);
                                   _context.Entry(existingItem).State = EntityState.Deleted;
                              }
                         }
                    }
                    foreach(var item in model.UserPhones) {
                         var existingItem = existingObject.UserPhones.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null)     //record present in DB, therefore update DB record with values from the model
                         {
                              if (item.CandidateId == 0) item.CandidateId = model.Id;
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {        //insert new record
                              var newObj = new UserPhone(existingObject.Id, item.MobileNo, item.IsMain);
                              existingObject.UserPhones.Add(newObj);
                              _context.Entry(newObj).State = EntityState.Added;
                         }
                    }
               }

               //UserQualifications
               if(model.UserQualifications != null) {
                    if (existingObject.UserQualifications != null) {
                         foreach(var existingItem in existingObject.UserQualifications.ToList()) {
                              if (!model.UserQualifications.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                              {
                                   _context.UserQualifications.Remove(existingItem);
                                   _context.Entry(existingItem).State = EntityState.Deleted;
                              }
                         }
                         foreach(var item in model.UserQualifications) {
                              if (item.CandidateId == 0) item.CandidateId = model.Id;
                              var existingItem = existingObject.UserQualifications.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingItem != null) {    //record present in DB, therefore update DB record with values from the model
                                   _context.Entry(existingItem).CurrentValues.SetValues(item);
                                   _context.Entry(existingItem).State = EntityState.Modified;
                              } else {         //insert new record
                                   var newObj = new UserQualification(existingObject.Id, item.QualificationId, item.IsMain);
                                   existingObject.UserQualifications.Add(newObj);
                                   _context.Entry(newObj).State = EntityState.Added;
                              }
                         }
                    }
               }

               //UserProfessions
               if(model.UserProfessions != null) {
                    if (existingObject.UserProfessions != null) {
                         foreach(var existingItem in existingObject.UserProfessions.ToList())
                         {
                              if (!model.UserProfessions.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                              {
                                   _context.UserProfessions.Remove(existingItem);
                                   _context.Entry(existingItem).State = EntityState.Deleted;
                              }
                         }
                         foreach(var item in model.UserProfessions) {
                              if (item.CandidateId == 0) item.CandidateId = model.Id;
                              var existingItem = existingObject.UserProfessions.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingItem != null) {    //record present in DB, therefore update DB record with values from the model
                                   _context.Entry(existingItem).CurrentValues.SetValues(item);
                                   _context.Entry(existingItem).State = EntityState.Modified;
                              } else {         //insert new record
                                   var newObj = new UserProfession(existingObject.Id, item.CategoryId, item.IndustryId, item.IsMain);
                                   existingObject.UserProfessions.Add(newObj);
                                   _context.Entry(newObj).State = EntityState.Added;
                              }
                         }
                    }
               }

               //UserPassports
               if (model.UserPassports != null) {
                    if (existingObject.UserPassports != null) {
                         foreach(var existingItem in existingObject.UserPassports.ToList())  {
                              if (!model.UserPassports.Any(c => c.Id == existingItem.Id && c.Id != default(int))) {
                                   _context.UserPassports.Remove(existingItem);
                                   _context.Entry(existingItem).State = EntityState.Deleted;
                              }
                         }

                         foreach(var item in model.UserPassports) {
                              if (item.CandidateId == 0) item.CandidateId = model.Id;
                              var existingItem = existingObject.UserPassports.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingItem != null) {     //record present in DB, therefore update DB record with values from the model
                                   _context.Entry(existingItem).CurrentValues.SetValues(item);
                                   _context.Entry(existingItem).State = EntityState.Modified;
                              } else {         //insert new record
                                   var newObj = new UserPassport(existingObject.Id, item.PassportNo, item.Nationality, item.Validity);
                                   existingObject.UserPassports.Add(newObj);
                                   _context.Entry(newObj).State = EntityState.Added;
                              }
                         }
                    }
               }

               //UserAExperiences
               if (model.UserExperiences != null) {
                    foreach(var existingItem in existingObject.UserExperiences.ToList()) {
                         if (!model.UserExperiences.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                         {
                              _context.UserExps.Remove(existingItem);
                              _context.Entry(existingItem).State = EntityState.Deleted;
                         }
                    }
                    foreach(var item in model.UserExperiences) {
                         if (item.CandidateId == 0) item.CandidateId = model.Id;
                         var existingItem = existingObject.UserExperiences.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null) {    //record present in DB, therefore update DB record with values from the model
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {         //insert new record
                              var nextSrNo = (await _context.UserExps.Where(x => x.CandidateId == existingObject.Id).MaxAsync(x => (int?)x.SrNo)) ?? 0;
                              ++nextSrNo;
                              var newObj = new UserExp(existingObject.Id, nextSrNo, item.PositionId, item.Employer, 
                                   item.Position, item.SalaryCurrency, (int)item.MonthlySalaryDrawn, item.WorkedFrom, 
                                   item.WorkedUpto);
                              existingObject.UserExperiences.Add(newObj);
                              _context.Entry(newObj).State = EntityState.Added;
                         }
                    }
               }
               /*
          
               if (UserFormFiles != null && UserFormFiles.Count > 0) {
                    var id = model.Id;
                    var attachments = new List<UserAttachment>();
                    foreach (var doc in UserFormFiles)
                    {
                         //check if file alredy exists, if so, delete it
                         //var existingItem = existingObject.UserAttachments.Where(c => c.url.ToLower() == doc.FileName.ToLower()).FirstOrDefault();
                         

                         var filePath = Path.Combine(@"App_Data", id.ToString(),  doc.ContentType, doc.FileName);
                         new FileInfo(filePath).Directory?.Create();

                         await using var stream = new FileStream(filePath, FileMode.Create);
                         await doc.CopyToAsync(stream);
                        // _logger.LogInformation($"The uploaded file [{doc.FileName}] is saved as [{filePath}].");
                         var newObj = new UserAttachment{url= filePath, AttachmentSizeInBytes=doc.Length, DateUploaded = DateTime.Now, 
                              AttachmentType= doc.ContentType, AppUserId = model.AppUserId, CandidateId = model.Id };
                         existingObject.UserAttachments.Add(newObj);
                         _context.Entry(newObj).State = EntityState.Added;
                         //result.Add(new UserAttachment { FileName = doc.FileName, FileSize = doc.Length });
                    }

               }
               */
               _context.Entry(existingObject).State = EntityState.Modified;

               if (await _context.SaveChangesAsync() > 0) return existingObject;
               return null;
          }

          public async Task<ICollection<CandidateCity>> GetCandidateCityNames()
          {
               var c = await _context.Candidates
                    .Select(x => x.City).Distinct() .ToListAsync();
               var lsts = new List<CandidateCity>();
               foreach(var lst in c)
               {
                    lsts.Add(new CandidateCity{City = lst});
               }
               return lsts;
          }

          public async Task<string> CheckPPNumberExists(string ppNumber)
          {
               var pp = await _context.Candidates.Where(x => x.PpNo.ToLower() == ppNumber.ToLower()).Select(x => new {x.ApplicationNo, x.FullName}).FirstOrDefaultAsync();
               if (pp==null) return null;
               return pp.ApplicationNo + " - " + pp.FullName;
          }

          public async Task<string> GetCategoryNameFromCategoryId(int id)
          {
               return await _context.Categories.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
          }

          public async Task<string> GetCustomerNameFromCustomerId(int id)
          {
               return await _context.Customers.Where(x => x.Id == id).Select(x => x.CustomerName).FirstOrDefaultAsync();
          }

          public async Task<CandidateBriefDto> GetCandidateByAppNo(int appno)
          {
               var cv = await _context.Candidates.Where(x => x.ApplicationNo == appno)
                    .Select(x => new CandidateBriefDto{
                         Id = x.Id, Gender = x.Gender, ApplicationNo = appno, 
                         FullName = x.FullName, City = x.City, ReferredById = x.ReferredBy,
                         AadharNo = x.AadharNo,
                         CandidateStatusName = Enum.GetName(typeof(EnumCandidateStatus), x.CandidateStatus)})
                    .FirstOrDefaultAsync();
               return cv;
          }

          public async Task<CandidateBriefDto> GetCandidateBriefById(int candidateid)
          {
               var cv = await _context.Candidates.Where(x => x.Id == candidateid)
                    .Select(x => new CandidateBriefDto{
                         Id = x.Id, Gender = x.Gender, ApplicationNo = x.ApplicationNo, 
                         FullName = x.FullName, City = x.City, ReferredById = x.ReferredBy,
                    })
                    .FirstOrDefaultAsync();
               return cv;
          }

     }
}