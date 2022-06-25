using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Admin;
using core.Entities.Users;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using core.Params;
using AutoMapper;
using core.Entities.HR;
using core.Entities.EmailandSMS;
using core.Entities.Orders;

namespace infra.Services
{
     public class UserHistoryService : IUserHistoryService
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICommonServices _commonServices;
          private readonly IMapper _mapper;
          public IUserService UserService { get; }
          private readonly IUserService _userService;
          private readonly IComposeMessageForCandidates _composeProspectiveMsg;
          private readonly IComposeMessagesForHR _composeHRMsg;
          public UserHistoryService(ATSContext context, IUnitOfWork unitOfWork, ICommonServices commonServices,  
               IUserService userService, IMapper mapper, IComposeMessageForCandidates composeProspectiveMsg, 
               IComposeMessagesForHR composeHRMsg)
          {
               _userService = userService;
               UserService = userService;
               _mapper = mapper;
               _commonServices = commonServices;
               _unitOfWork = unitOfWork;
               _context = context;
               _composeProspectiveMsg = composeProspectiveMsg;
               _composeHRMsg = composeHRMsg;
          }

           public async Task<UserHistory> AddUserContact(UserHistory userContact)
          {
               if (userContact.PersonId !=0) {
                    var user = await _context.Candidates.FindAsync(userContact.PersonId);
                    if (user == null) return null;
                    userContact.PersonName = user.FullName;
               }
               _unitOfWork.Repository<UserHistory>().Add(userContact);
               
               if (await _unitOfWork.Complete() > 0) return userContact;

               return null;
          }
          
          public async Task<bool> DeleteUserContact(UserHistory userContact)
          {
               var contact = await _context.UserHistories.FindAsync(userContact.Id);
               if (contact == null) throw new Exception("invalid object");

               _unitOfWork.Repository<UserHistory>().Delete(userContact);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> DeleteUserContactById (int userContactId)
          {
               var contact = await _context.UserHistories.FindAsync(userContactId);
               if (contact == null) throw new Exception("invalid object");

               _unitOfWork.Repository<UserHistory>().Delete(contact);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<ContactResult>> GetContactResults()
          {
               return await _context.ContactResults.OrderBy(x => x.PersonType).ThenBy(x => x.Name).ToListAsync();
          }

        
          public async Task<UserHistory> GetHistoryFromHistoryId(int historyId)
          {
               var history = await _context.UserHistories.Where(x => x.Id == historyId).Include(x => x.UserHistoryItems).FirstOrDefaultAsync();
               return history;     //_mapper.Map<UserHistory, UserHistoryDto>(history);
          }
          private async Task<UserHistory> GetUserHistory(UserHistoryParams histParams)
          {
               var qry = _context.UserHistories.AsQueryable();
               if (histParams.ApplicationNo.HasValue) {
                    qry =  qry.Where(x =>x.ApplicationNo == histParams.ApplicationNo);
               } else {
                    if (!string.IsNullOrEmpty(histParams.MobileNo)) qry = qry.Where(x => x.PhoneNo == histParams.MobileNo);
                    if (!string.IsNullOrEmpty(histParams.EmailId)) qry = qry.Where(x => x.EmailId.ToLower() == histParams.EmailId.ToLower());
                    if(histParams.PersonId.HasValue) qry = qry.Where(x => x.PersonId==histParams.PersonId);
               }
               qry = qry.Include(x => x.UserHistoryItems);
               var q = await qry.FirstOrDefaultAsync();
               return q;
          }


          public async Task<UserHistory> GetOrAddUserHistoryOfProspectiveCandidatesByParams(ProspectiveCandidateParams prospectiveParams)
          {
               //check if the object has empty elements
               
               var cv = new Candidate();
               var history = new UserHistory();
               var histParams = new UserHistoryParams{DateAdded = prospectiveParams.DateAdded, CategoryRef = prospectiveParams.CategoryRef, PersonType = "prospective"};
               history = await GetUserHistory(histParams);

               if (history !=null) return history;     

               //no record in UserHistory, so create new UserHistory after verying input data;
               var userHist = new UserHistory();
               
               var phn = histParams.MobileNo ?? "";
               
               var qryCand = _context.ProspectiveCandidates.AsQueryable();
               if (histParams.PersonId.HasValue) qryCand = qryCand.Where(x => x.Id == histParams.PersonId);
               if (!string.IsNullOrEmpty(histParams.EmailId )) qryCand = qryCand.Where(x => x.Email.ToLower() == histParams.EmailId.ToLower());                         
               if (!string.IsNullOrEmpty(histParams.MobileNo)) qryCand = qryCand.Where(x => x.PhoneNo.Contains(histParams.MobileNo) || x.AlternatePhoneNo.Contains(histParams.MobileNo));
               
               var cand = await qryCand.FirstOrDefaultAsync();
               
               if(cand == null) return null;

               userHist = new UserHistory{PersonType= "prospective", PersonName = cand.CandidateName, PersonId = cand.Id,
                    EmailId = cand.Email, CreatedOn = DateTime.Now, PhoneNo = cand.PhoneNo};

               _unitOfWork.Repository<UserHistory>().Add(userHist);

               await _unitOfWork.Complete();
                    
               history = await GetUserHistory(histParams);
               if(history==null) return null;

               if(history.UserHistoryItems != null) {
                    foreach(var item in history.UserHistoryItems) {
                         if (item.LoggedInUserName == "") item.LoggedInUserName = await _commonServices.GetEmployeeNameFromEmployeeId(item.LoggedInUserId);
                         if (item.ContactResultId > 0) item.ContactResultName = Enum.GetName(typeof(EnumContactResult), item.ContactResultId);
                    }
               }
               return history;
          }

          public async Task<UserHistory> GetOrAddUserHistoryByParams(UserHistoryParams histParams)
          {
               //check if the object has empty elements
               var hist = new UserHistory();

               if(histParams.PersonType == "prospective") {
                    hist = await _context.UserHistories.Where(x => x.PersonId == histParams.PersonId && x.PersonType=="prospective").Include(x => x.UserHistoryItems).FirstOrDefaultAsync();
                    if (hist != null) return hist;
                    
                    var prosp = await _context.ProspectiveCandidates.FindAsync(histParams.PersonId);
                    if (prosp == null) return null;
                    hist  = new UserHistory{PersonType= "prospective", PersonName = prosp.CandidateName, PersonId = prosp.Id,
                         EmailId = prosp.Email, CreatedOn = DateTime.Now, PhoneNo = prosp.PhoneNo};
               } else {
               var cv = new Candidate();
               hist = await GetUserHistory(histParams);

               if (hist !=null) return hist;     

               //no record in UserHistory, so create new UserHistory after verying input data;
               
               //based upon persontype, get person details either from customer or candidate
               //and create new history object
               var userHist = new UserHistory();
                    var phn = histParams.MobileNo ?? "";

                    //first, try to read candidates records, as probability of calls to/from candidates are high due to its volume, almost > 95%
                    var qryCand = _context.Candidates.AsQueryable();
                    if (histParams.PersonId.HasValue) qryCand = qryCand.Where(x => x.Id == histParams.PersonId);
                    if (!string.IsNullOrEmpty(histParams.EmailId )) qryCand = qryCand.Where(x => x.Email.ToLower() == histParams.EmailId.ToLower());                         
                    if (!string.IsNullOrEmpty(histParams.MobileNo)) qryCand = qryCand.Where(x => x.UserPhones.Any(x => x.MobileNo == phn));
                    qryCand = qryCand.Include(x => x.UserPhones);
                    var cand = await qryCand.FirstOrDefaultAsync();
                    
                    if(cand == null) {       //try to fetch records from customer officials
                         var qryC = _context.CustomerOfficials
                              .Join(_context.Customers, o => o.CustomerId, c => c.Id, (o, c) => new {official = o, customer=c})
                          .AsQueryable();

                         if (histParams.PersonId.HasValue) qryC = qryC.Where(x => x.official.Id == histParams.PersonId);
                         if (!string.IsNullOrEmpty(histParams.EmailId)) qryC = qryC.Where(x => x.official.Email.ToLower() == histParams.EmailId.ToLower());
                         if (!string.IsNullOrEmpty(histParams.MobileNo)) qryC = qryC.Where(x => x.official.Mobile == phn);
                         //if (histParams.PersonId.HasValue) hist = hist.Where(x => x.Id == histParams.PersonId);
                         var off = await qryC.Select(x => new {x.official.Id, x.official.CustomerId, x.official.OfficialName, x.customer.CustomerName, 
                              x.official.Mobile, x.customer.CustomerType, x.official.Email}).FirstOrDefaultAsync();
                         if(off==null) return null;

                         hist = new UserHistory{PersonType="official", PersonName = off.OfficialName, PersonId = off.CustomerId,
                              PhoneNo = off.Mobile, EmailId = off.Email, CreatedOn = DateTime.Now};
                    } else {
                         
                         //cand != null.  If the histParam has emailId and phoneNo empty, then check if UserHistory has records
                         //that have these values.  Bcz emailId and PhoneNo in UserHistory are unique.
                         /*however above should never happen, so this block is commented out
                         if(string.IsNullOrEmpty(histParams.EmailId) && !string.IsNullOrEmpty(cand.Email) ) {
                              hist = await _context.UserHistories.Where(x => x.EmailId.ToLower()==cand.Email.ToLower())
                                   .Include(x => x.UserHistoryItems).FirstOrDefaultAsync();
                         };
                         if(hist==null) {
                              if(string.IsNullOrEmpty(histParams.MobileNo) && !string.IsNullOrEmpty(cand.UserPhones.Select(x => x.MobileNo).FirstOrDefault())) {
                              hist = await _context.UserHistories.Where(x => x.PhoneNo==cand.UserPhones.Select(x => x.MobileNo).FirstOrDefault())
                                   .Include(x => x.UserHistoryItems).FirstOrDefaultAsync();
                         }
                         if(hist==null) {
                         */
                              hist = new UserHistory{PersonType= "candidate", PersonName = cand.FullName, PersonId = cand.Id,
                                   EmailId = cand.Email, CreatedOn = DateTime.Now, ApplicationNo = cand.ApplicationNo,
                                   PhoneNo = cand.UserPhones == null || cand.UserPhones.Count==0 ? "" : cand.UserPhones.Select(x => x.MobileNo).FirstOrDefault()};
                         //}
                    }  
               }
               
               _unitOfWork.Repository<UserHistory>().Add(hist);
              
               await _unitOfWork.Complete();
               
                    
               hist = await GetUserHistory(histParams);
               if(hist==null) return null;

                    //var historyDto = _mapper.Map<UserHistory, UserHistoryDto>(history);

               if(hist.UserHistoryItems != null) {
                    foreach(var item in hist.UserHistoryItems) {
                         if (item.LoggedInUserName == "") item.LoggedInUserName = await _commonServices.GetEmployeeNameFromEmployeeId(item.LoggedInUserId);
                         if (item.ContactResultId > 0) item.ContactResultName = Enum.GetName(typeof(EnumContactResult), item.ContactResultId);
                    }
               }
               return hist;
          }

          public async Task<bool> EditContactHistoryItems(ICollection<UserHistoryItem> model, int loggedinEmpId)
          {
               var existingItems = await _context.UserHistoryItems
                    .Where(x => x.UserHistoryId == model.Select(x => x.UserHistoryId).FirstOrDefault())
                    .AsNoTracking()
                    .ToListAsync();
               
               foreach(var existingItem in existingItems) {
                    if (!model.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.UserHistoryItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               if(model != null) {
                    foreach(var modelItem in model) {
                         var existingItem = existingItems.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                         if(existingItem != null) {
                              _context.Entry(existingItem).CurrentValues.SetValues(existingItem);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {
                              var newItem = new UserHistoryItem(modelItem.UserHistoryId, modelItem.PhoneNo, 
                                   modelItem.DateOfContact.Year < 2000 ? DateTime.Now : modelItem.DateOfContact, 
                                   loggedinEmpId, modelItem.Subject, modelItem.CategoryRef, modelItem.ContactResultId, 
                                   modelItem.ContactResultName, modelItem.ComposeEmailMessage, modelItem.GistOfDiscussions);
                              
                              existingItems.Add(newItem);
                              _context.Entry(newItem).State = EntityState.Added;
                         }
                    }
               }

               var succeeded = await _context.SaveChangesAsync() > 0;
               return succeeded;

          }

          public async Task<UserHistoryReturnDto> EditContactHistory(UserHistory model, LoggedInUserDto userDto)
          {
               var historyReturnDto = new UserHistoryReturnDto();

               var existingHistory = await _context.UserHistories
                    .Where(x => x.Id == model.Id)
                    .Include(x => x.UserHistoryItems)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
               
               if (existingHistory == null) {
                    historyReturnDto.Succeeded=false;
                    return historyReturnDto;
               }
               _context.Entry(existingHistory).CurrentValues.SetValues(model);

               //save NewHistoryItem for use in composeMessages
               var historyItemForMessage = new UserHistoryItem();

               foreach(var existingHistoryItem in existingHistory.UserHistoryItems.ToList())
               {
                    if (!model.UserHistoryItems.Any(c => c.Id == existingHistoryItem.Id && 
                         c.Id != default(int)))
                    {
                         _context.UserHistoryItems.Remove(existingHistoryItem);
                         _context.Entry(existingHistoryItem).State = EntityState.Deleted;
                    }
               }

               var historyItemForMessages = new List<UserHistoryItem>();

               if(model.UserHistoryItems != null) {
                    foreach(var modelHistoryItem in model.UserHistoryItems)
                    {
                         var existingModelItem = existingHistory.UserHistoryItems
                              .Where(c => c.Id == modelHistoryItem.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingModelItem != null)
                         {
                              _context.Entry(existingModelItem).CurrentValues.SetValues(modelHistoryItem);
                              _context.Entry(existingModelItem).State = EntityState.Modified;
                         } else {
                              var newHistoryItem = new UserHistoryItem(model.Id, modelHistoryItem.PhoneNo, 
                                   modelHistoryItem.DateOfContact.Year < 2000 ? DateTime.Now : modelHistoryItem.DateOfContact, 
                                   userDto.LoggedInEmployeeId, modelHistoryItem.Subject, modelHistoryItem.CategoryRef, 
                                   modelHistoryItem.ContactResultId, modelHistoryItem.ContactResultName, 
                                   modelHistoryItem.ComposeEmailMessage, modelHistoryItem.GistOfDiscussions);
                                   
                              existingHistory.UserHistoryItems.Add(newHistoryItem);
                              _context.Entry(newHistoryItem).State = EntityState.Added;
                              if(modelHistoryItem.ComposeEmailMessage) historyItemForMessages.Add(newHistoryItem);
                         }
                    }
               }

               var messages = new List<EmailMessage>();
               if(historyItemForMessages.Count > 0) {
                    messages = (List<EmailMessage>)await ComposeMessages(historyItemForMessages, userDto, model.PersonType, model.PersonId);
               }
               //update userhistory.status based upon last item status
               string currentStatus="";
               string username="";
               string dt="";

               var lastRecord = model.UserHistoryItems.OrderByDescending(x => x.DateOfContact).Take(1)
                    .Select(x => new {x.ContactResultId, x.ContactResultName, x.LoggedInUserName, x.DateOfContact}).FirstOrDefault();
               if (lastRecord != null && string.IsNullOrEmpty(lastRecord.ContactResultName) ) {
                    if(string.IsNullOrEmpty(lastRecord.ContactResultName)) {
                         var temp = await _context.ContactResults.FindAsync(lastRecord.ContactResultId);
                         currentStatus = temp.Name;
                    }
               } else {
                    currentStatus = lastRecord.ContactResultName;
               }
               
               username=lastRecord.LoggedInUserName;
               dt=lastRecord.DateOfContact.ToShortDateString();
          
               if(model.PersonType.ToLower()=="prospective") {
                    var prospective = await _context.ProspectiveCandidates.FindAsync(model.PersonId);
                    if(prospective != null) {
                         prospective.Status = currentStatus;
                         prospective.UserName=username;
                         prospective.StatusDate=Convert.ToDateTime(dt);
                         _context.Entry(prospective).State = EntityState.Modified;
                    }
               }
               _context.Entry(existingHistory).State = EntityState.Modified;

               foreach(var msg in messages) {
                    _context.Entry(msg).State = EntityState.Added;
               }
               historyReturnDto.Succeeded = await _context.SaveChangesAsync() > 0;
               historyReturnDto.MessageCount = messages.Count;
               return historyReturnDto;
          }
     
          private async Task<ICollection<EmailMessage>> ComposeMessages(List<UserHistoryItem> historyItemForMessages, LoggedInUserDto userDto, string PersonType, int PersonId) 
          {
               var msg = new EmailMessage();
               var messages = new List<EmailMessage>();
               var modelids = new List<int>();
               var prospectiveObj = new ComposeMessageDtoForProspects();

               if (historyItemForMessages.Count == 0) return null;

               //get candidate or prospect variables
               if(PersonType.ToLower()=="prospective")  {
                    var prospect = await _context.ProspectiveCandidates.FindAsync(PersonId);
                    if(prospect==null) {
                         //returnDto.ErrorMessage="invalid prospective PersonId";
                         return null;
                    }
                    prospectiveObj = new ComposeMessageDtoForProspects(prospect.Id, prospect.CandidateName, prospect.Email, prospect.PhoneNo, 
                         prospect.Source, prospect.City, "");
               } else if(PersonType.ToLower()=="candidate") {
                    prospectiveObj = await (from c in _context.Candidates where c.Id == PersonId 
                         join p in _context.UserPhones on c.Id equals p.CandidateId into phones
                         from ph in phones.DefaultIfEmpty()
                         join a in _context.EntityAddresses on c.Id equals a.CandidateId into address 
                         from ad in address.DefaultIfEmpty()
                         select new ComposeMessageDtoForProspects(c.Id, c.FullName, c.Email, ph.MobileNo ?? "",  c.Source, ad.City ?? "", 
                              ad.Country ?? "")).FirstOrDefaultAsync();
               }
          
               
               foreach(var item in historyItemForMessages)
               {
                    if (!item.ComposeEmailMessage) continue;
                    var orderitemdetails = await OrderItemIdFromCategoryRef(item.CategoryRef);
                    if(orderitemdetails == null) continue;
                    if(string.IsNullOrEmpty(prospectiveObj.CategoryName)) prospectiveObj.CategoryName=orderitemdetails.CategoryName;
                    if(string.IsNullOrEmpty(prospectiveObj.City)) prospectiveObj.City=orderitemdetails.City;
                    if(string.IsNullOrEmpty(prospectiveObj.Country)) prospectiveObj.Country=orderitemdetails.Country;

                    modelids.Add(PersonId);
                    
                    switch(item.ContactResultId) {
                         case (int)EnumContactResult.Interested:
                              msg = _composeProspectiveMsg.ComposeMessagesForConsentOfInterest(prospectiveObj, userDto);
                              break;
                         case (int)EnumContactResult.PhoneNotReachable:
                         case (int)EnumContactResult.PhoneOutOfNetwork:
                         case (int)EnumContactResult.PhoneUnanswered:
                         case (int)EnumContactResult.WrongNumber:
                              msg = _composeProspectiveMsg.ComposeMessagesForFailureToReach(prospectiveObj, userDto);
                              messages.Add(msg);
                              break;
                         default:
                              break;
                    }
               
                    
               }
               
               return messages;
          }
          public async Task<CandidateBriefDto> GetCandidateBriefByParams(CandidateSpecParams SpecParams)
          {
               var cand = new Candidate();

               if(SpecParams.ApplicationNoFrom != 0) {
                    var c = await _context.Candidates.Where(x => x.ApplicationNo == SpecParams.ApplicationNoFrom)
                         .Select(x => new CandidateBriefDto(x.Id, x.Gender, x.ApplicationNo, x.AadharNo, x.FullName,
                              x.City, x.ReferredBy, ""))
                         .FirstOrDefaultAsync();
                    return c;
               } else if (SpecParams.CandidateId != 0) {
                    var c = await _context.Candidates.Where(x => x.Id == SpecParams.CandidateId)
                         .Select(x => new CandidateBriefDto(x.Id, x.Gender, x.ApplicationNo, x.AadharNo, x.FullName,
                              x.City, x.ReferredBy, ""))
                         .FirstOrDefaultAsync();
                    return c;
               } 
               return null;
          }

          public async Task<UserHistory> GetHistoryByCandidateId(int candidateid)
          {
               var history = await _context.UserHistories.Where(x => x.PersonId == candidateid && x.PersonType.ToLower() == "candidate")
                    .Include(x => x.UserHistoryItems)
                    .FirstOrDefaultAsync();

               if (history == null) {
                    var cv = await _context.Candidates.Where(x => x.Id == candidateid)
                         .Select(x => new {id=x.Id, name=x.FullName, appno=x.ApplicationNo, emailid=x.Email, ph=x.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault()})
                         .FirstOrDefaultAsync();
                    if (cv == null) return null;
                    history = new UserHistory{ApplicationNo=cv.appno, PersonId=cv.id, PersonName=cv.name, PersonType="candidate", PhoneNo=cv.ph, EmailId=cv.emailid};
                    _unitOfWork.Repository<UserHistory>().Add(history);
                    if (await _unitOfWork.Complete() == 0) return null;
               }
               
               //var historyDto = _mapper.Map<UserHistory, UserHistoryDto>(history);

               if(history.UserHistoryItems != null) {
                    foreach(var item in history.UserHistoryItems) {
                         if (item.LoggedInUserName == "") item.LoggedInUserName = await _commonServices.GetEmployeeNameFromEmployeeId(item.LoggedInUserId);
                         if (item.ContactResultId > 0) item.ContactResultName = Enum.GetName(typeof(EnumContactResult), item.ContactResultId);
                    }
               }

               return history;
          }

          private async Task<OrderItemIdCityCountryDto> OrderItemIdFromCategoryRef(string categoryref)
          {
               if(string.IsNullOrEmpty(categoryref)) return null;
               
               int i = categoryref.IndexOf("-");
               if (i== -1) return null;
               var orderno = categoryref.Substring(0,i);
               var srno = categoryref.Substring(i+1);
               if (string.IsNullOrEmpty(orderno) || string.IsNullOrEmpty(srno)) return null;
               int iorderno = Convert.ToInt32(orderno);
               int isrno = Convert.ToInt32(srno);

               var qry = await (from o in _context.Orders where o.OrderNo == iorderno 
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join item in _context.OrderItems on o.Id equals item.OrderId 
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    select new OrderItemIdCityCountryDto{OrderItemId=item.Id, CategoryName=cat.Name, 
                    City=c.City, Country=c.Country}).FirstOrDefaultAsync();
               if(qry==null) return null;
               return qry;
          }

          public async Task<ICollection<CategoryRefDto>> GetCategoryRefDetails()
          {
               var qry = await (from i in _context.OrderItems where i.Status != (int)EnumOrderItemStatus.Concluded
                    join o in _context.Orders on i.OrderId equals o.Id where o.Status != EnumOrderStatus.Concluded
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new CategoryRefDto {
                         OrderNo = o.OrderNo, SrNo=i.SrNo, 
                         CategoryRef = o.OrderNo + "-" + i.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName
                    }).ToListAsync();
               
               return qry;
          }
     }
}