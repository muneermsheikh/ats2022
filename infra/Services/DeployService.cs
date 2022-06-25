using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;


// TODO - constants for deploymentStageIds
namespace infra.Services
{
     public class DeployService : IDeployService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly IMapper _mapper;
          private readonly ICommonServices _commonServices;
          public DeployService(IUnitOfWork unitOfWork, ATSContext context, IMapper mapper, ICommonServices commonServices)
          {
               _commonServices = commonServices;
               _mapper = mapper;
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<Pagination<CVRefAndDeployDto>> GetPendingDeployments(DeployParams deployParams)
          {
               /* var qry = _context.CVRefs.Where(x => x.RefStatus == (int)EnumCVRefStatus.Selected)
                    .Include(x => x.Candidate)
                    .Include(x => x.Deploys).Include(x => x.OrderItem).ThenInclude(x => x.Category)
                    .Select(x => new {OrderId = x.OrderItem.OrderId, OrderNo = x.OrderNo, 
                         ApplicationNo = x.ApplicationNo, CandidateId = x.CandidateId, CandidateName = x.CandidateName, 
                         OrderItemId = x.OrderItemId, CategoryName = x.OrderItem.Category.Name, 
                         CategoryRef = x.OrderNo + "-" + x.OrderItem.SrNo + "-" + x.OrderItem.Category.Name,
                         CustomerName = x.CustomerName, Deploys = x.Deploys, CVRefId = x.Id, ReferredOn = x.ReferredOn,
                         SelectedOn = x.RefStatusDate, RefStatus = x.RefStatus
                         })
                    .OrderBy(x => x.OrderItemId).ThenBy(x => x.CVRefId)
                    .AsQueryable();
               */
          
               var qry = (from r in _context.CVRefs where r.RefStatus == (int)EnumCVRefStatus.Selected
                    join i in _context.OrderItems on  r.OrderItemId equals i.Id
                    join st in _context.DeployStatus on r.DeployStageId equals st.StageId
                    join c in _context.Candidates on r.CandidateId equals c.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    join cust in _context.Customers on o.CustomerId equals cust.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    orderby r.OrderItemId, r.Id 
                    
                    select new CVRefAndDeployDto{
                         Checked=false, CVRefId = r.Id, CustomerName = cust.CustomerName, 
                         OrderId = i.OrderId, OrderNo = o.OrderNo, OrderDate = o.OrderDate,
                         OrderItemId = r.OrderItemId,
                         CategoryRef = o.OrderNo + "-" + i.SrNo,
                         CategoryName = cat.Name, CustomerId = o.CustomerId,
                         CandidateId = c.Id, ApplicationNo=c.ApplicationNo,
                         CandidateName = c.FullName, ReferredOn = r.ReferredOn, SelectedOn = r.RefStatusDate,
                         RefStatus = r.RefStatus,
                         DeployStageName = st.StatusName,
                         DeployStageDate = (DateTime)r.DeployStageDate,
                         NextStageId = st.NextStageId,
                         NextStageDate = DateTime.Now
                    }
               ).AsQueryable();
               
               if(deployParams.OrderItemId > 0) qry = qry.Where(x => x.OrderItemId == deployParams.OrderItemId);
               if(deployParams.OrderId > 0) qry = qry.Where(x => x.OrderId == deployParams.OrderId);
               if (deployParams.CVRefId > 0) qry = qry.Where(x => x.CVRefId == deployParams.CVRefId);
               //if(deployParams.CustomerId > 0) qry=qry.Where(x => x.CustomerId == deployParams.CustomerId);
               
               var count = await qry.CountAsync();
               if (count == 0) return null;

               var data = await qry.Skip((deployParams.PageIndex-1)*deployParams.PageSize).Take(deployParams.PageSize).ToListAsync();
               /* var deployHeaders = new List<CVRefAndDeployDto>();
               var candidates = new List<CandidateSelected>();
               int lastOrderItemId=0;
               var Sels = new List<CandidateSelected>();
               var deployHeader = new CVRefAndDeployDto();

               foreach(var d in data) {
                    if (d.OrderItemId != lastOrderItemId) {
                         deployHeader = new CVRefAndDeployDto{
                              OrderId = d.OrderId, OrderNo = d.OrderNo, CompanyName = d.CustomerName,
                              OrderItemId = d.OrderItemId, CategoryRef = d.CategoryRef, CategoryName = d.CategoryName};
                         deployHeader.CompanyName=d.CustomerName;
                         lastOrderItemId=d.OrderItemId;
                         deployHeader.SelectedCandidates = new List<CandidateSelected>();
                         lastOrderItemId = d.OrderItemId;
                    } 
                    var candidate = new CandidateSelected{
                         Checked=false, CVRefId = d.CVRefId, CandidateId = d.CandidateId, ApplicationNo=d.ApplicationNo,
                         CandidateName = d.CandidateName, ReferredOn = d.ReferredOn, SelectedOn = d.SelectedOn,
                         SelectionId = 0, RefStatus = (EnumCVRefStatus)d.RefStatus,
                         Deploys = _mapper.Map<ICollection<Deploy>, ICollection<DeployDto>>(d.Deploys)
                    };
                    deployHeader.SelectedCandidates.Add(candidate);
                    deployHeaders.Add(deployHeader);
                    //candidates.Add(d.Candidates);
                    //deployHeader.SelectedCandidates=candidates;
               }
               */

               return new Pagination<CVRefAndDeployDto>(deployParams.PageIndex, deployParams.PageSize, count, data);
          }

          
          public async Task<int> CountOfPendingDeployments()
          {
               return await _context.CVRefs.Where(x => x.DeployStageId < (int)EnumDeployStatus.Concluded).CountAsync();
          }

          public async Task<DeployAddedDto> AddDeploymentTransaction(int cvrefId, int loggedInEmployeeId, EnumDeployStatus stageId, DateTime? transDate)
          {
               // A - if transDate missing, make it current date
               // B - Create a model based on input parameters,
               // C - verify stageId sequence, and update NextSTageId and NextSTageEstiamted date values
               // D - update DB
               // E - update CVRef.DeployStage values
               // F - Issue tasks for next process
               // G - Update Candidate.CandidateStatus to "Under Process" or "Traveled"

               DateTime dt;
               
               // A - 
               if ((transDate.HasValue && Convert.ToDateTime(transDate).Year < 2000) || !transDate.HasValue) { 
                    dt = DateTime.Now;
               } else {
                    dt = Convert.ToDateTime(transDate);
               }

               // B -
               var deploy = new Deploy(cvrefId, dt, stageId);

               // C - verify stageId sequence, and update NextSTageId and NextSTageEstiamted date values
               deploy = await VerifyModelStageInSeqAndUpdateNextStage(deploy);       //also updates nexTStageId and dAte
               if (deploy == null) return null;

               // D - update DB
               _unitOfWork.Repository<Deploy>().Add(deploy);

               // E - update CVRef.Deploy fields
               var cvref = await _context.CVRefs.FindAsync(deploy.CVRefId);
               cvref.DeployStageId = (int)deploy.StageId;
               cvref.DeployStageDate = deploy.TransactionDate;
               _unitOfWork.Repository<CVRef>().Update(cvref);

               // F - issue tasks
               var cvreviewid = await _context.CVRefs.Where(x => x.Id == deploy.CVRefId).Select(x => x.CVReviewId).FirstOrDefaultAsync();
               EnumTaskType thisTaskType=EnumTaskType.None;
               int AssignedToId=0;
               string candidateStatus="";
               var commondata = await _commonServices.CommonDataFromCVRefId(deploy.CVRefId);
               switch((EnumDeployStatus)deploy.NextStageId)
               {
                    case EnumDeployStatus.Selected:
                         thisTaskType = EnumTaskType.OfferLetterAcceptance;
                         AssignedToId = commondata.HRExecId;
                         candidateStatus="Selected";
                         break;

                    case EnumDeployStatus.OfferLetterAccepted:
                         thisTaskType = EnumTaskType.MedicalTestsMobilization;
                         AssignedToId = commondata.MedicalProcessInchargeEmpId;
                         candidateStatus="Under process - offer accepted";
                         break;

                    case EnumDeployStatus.ReferredForMedical:
                         thisTaskType = EnumTaskType.MedicallyFit;
                         AssignedToId = commondata.VisaProcessInchargeEmpId;
                         candidateStatus="Under process - Under Medical Tests";
                         break;

                    /* case EnumDeployStatus.MedicallyFit:
                         thisTaskType = EnumTaskType.VisaDocsKSACompilation;
                         AssignedToId = commondata.EmigProcessInchargeId;
                         candidateStatus="Under deployment - Medically Fit";
                         break;

                    case EnumDeployStatus.MedicallyUnfit:
                         thisTaskType = EnumTaskType.None;
                         candidateStatus = "Not Available - Medically Unfit";
                         break;
                    */
                    case EnumDeployStatus.VisaDocsPrepared:
                         thisTaskType = EnumTaskType.VisaDocSubmission;
                         AssignedToId = commondata.VisaProcessInchargeEmpId;
                         candidateStatus="Under process";
                         break;
                    /*
                    case EnumDeployStatus.VisaDocsSubmitted:
                         thisTaskType = EnumTaskType.VisaReceived;
                         AssignedToId = commondata.EmigProcessInchargeId;
                         break;

                    case EnumDeployStatus.VisaQueryRaised:
                         thisTaskType = EnumTaskType.VisaReceived;
                         AssignedToId = commondata.EmigProcessInchargeId;
                         candidateStatus = "Under Process";
                         break;
                    */
                    case EnumDeployStatus.VisaReceived:
                         thisTaskType = EnumTaskType.EmigrationAppLodging;
                         AssignedToId = commondata.TravelProcessInchargeId;
                         candidateStatus = "Under process - visa endorsed";
                         break;
                    /*
                    case EnumDeployStatus.VisaDenied:
                         thisTaskType = EnumTaskType.None;
                         break;
                    */
                    case EnumDeployStatus.EmigDocsLodgedOnLine:
                         thisTaskType = EnumTaskType.EmigrationDocsSubmitted;
                         AssignedToId = commondata.TravelProcessInchargeId;
                         candidateStatus="under process";
                         break;
                    /*
                    case EnumDeployStatus.EmigDocsPPSubmitted:
                         thisTaskType = EnumTaskType.EmigrationGranted;

                         break;

                    case EnumDeployStatus.EmigDocsQueryRecd:
                         thisTaskType = EnumTaskType.EmigrationDocsSubmitted;
                         break;

                    case EnumDeployStatus.EmigrationDenied:
                         thisTaskType = EnumTaskType.None;
                         break;
                    */
                    case EnumDeployStatus.EmigrationGranted:
                         thisTaskType = EnumTaskType.EmigrationGranted;
                         candidateStatus="Under process - Emigration Cleared";
                         break;
                    
                    case EnumDeployStatus.TravelTicketBooked:
                         thisTaskType = EnumTaskType.Traveled;
                         candidateStatus="Travel Booked";
                         break;

                    case EnumDeployStatus.Traveled:
                         thisTaskType = EnumTaskType.ArrivalAcknowledgedByClient;
                         candidateStatus="Traveled";
                         break;
                    /*
                    case EnumDeployStatus.TravelCanceled:
                         thisTaskType = EnumTaskType.None;
                         break;
                    */
                    case EnumDeployStatus.ArrivalAcknowledgedByClient:
                         thisTaskType = EnumTaskType.None;
                         break;
                    default:
                         break;
                    
               }
               
               if(AssignedToId !=0)
               {
                    var task = new ApplicationTask((int)thisTaskType, dt, loggedInEmployeeId, AssignedToId,
                         commondata.OrderId, commondata.OrderNo, commondata.OrderItemId, "Task for you to organize " +
                         ProcessName(deploy.NextStageId) + " for " + commondata.CandidateDesc, deploy.NextEstimatedStageDate,
                         "Open", commondata.CandidateId, cvreviewid);
                    _unitOfWork.Repository<ApplicationTask>().Add(task);
               }

               //update candidate.CandidateStatus
               var candidate = await _context.Candidates.FindAsync(cvref.CandidateId);
               candidate.CandidateStatus = candidateStatus;
               _unitOfWork.Repository<Candidate>().Update(candidate);

               if (await _unitOfWork.Complete() > 0) return _mapper.Map<Deploy, DeployAddedDto>(deploy);

               return null;
          }

          public async Task<ICollection<DeployAddedDto>> AddDeploymentTransactions(ICollection<DeployPostDto> deployPosts, int loggedInEmployeeId)
          {
               // A - if transDate missing, make it current date
               // B - Create a model based on input parameters,
               // C - verify stageId sequence, and update NextSTageId and NextSTageEstiamted date values
               // D - update DB
               // E - update CVRef.DeployStage values
               // F - Issue tasks for next process
               // g - uPATE CANDIDATE.STATUS

               DateTime dt;
               var deploys = new List<Deploy>();
               foreach(var post in deployPosts)
               {
                    dt = post.TransactionDate;

                    // B -
                         var deploy = new Deploy(post.CVRefId, dt, post.StageId);

                    // C - verify stageId sequence, and update NextSTageId and NextSTageEstiamted date values
                         deploy = await VerifyModelStageInSeqAndUpdateNextStage(deploy);       //also updates nexTStageId and dAte
                         if (deploy == null) return null;

                    // D - update DB
                         _unitOfWork.Repository<Deploy>().Add(deploy);
                         deploys.Add(deploy);

                    // E - update CVRef.Deploy fields
                         var cvref = await _context.CVRefs.FindAsync(deploy.CVRefId);
                         cvref.DeployStageId = (int)deploy.StageId;
                         cvref.DeployStageDate = deploy.TransactionDate;
                         _unitOfWork.Repository<CVRef>().Update(cvref);

                    // F - issue tasks
                         var cvreview = await _context.CVRefs.Where(x => x.Id == deploy.CVRefId).Select(x => new {x.CVReviewId, x.CandidateId})
                              .FirstOrDefaultAsync();
                         EnumTaskType thisTaskType=EnumTaskType.None;
                         int AssignedToId=0;
                         string candidateStatus = "";
                         var commondata = await _commonServices.CommonDataFromCVRefId(deploy.CVRefId);
                         switch(deploy.NextStageId)
                         {
                              case EnumDeployStatus.Selected:
                                   thisTaskType = EnumTaskType.OfferLetterAcceptance;
                                   AssignedToId = commondata.HRExecId;
                                   break;

                              case EnumDeployStatus.OfferLetterAccepted:
                                   thisTaskType = EnumTaskType.MedicalTestsMobilization;
                                   AssignedToId = commondata.MedicalProcessInchargeEmpId;
                                   candidateStatus="Under process - Offer Accepted";
                                   break;

                              case EnumDeployStatus.ReferredForMedical:
                                   thisTaskType = EnumTaskType.ReferredForMedicalTest;;
                                   AssignedToId = commondata.VisaProcessInchargeEmpId;
                                   candidateStatus="Under process - under medical test";
                                   break;

                              /* case EnumDeployStatus.MedicallyFit:
                                   thisTaskType = EnumTaskType.VisaDocsKSACompilation;
                                   candidateStatus = "Under process - medically fit";
                                   AssignedToId = commondata.EmigProcessInchargeId;
                                   break;

                              case EnumDeployStatus.MedicallyUnfit:
                                   thisTaskType = EnumTaskType.None;
                                   break;
                              */
                              case EnumDeployStatus.VisaDocsPrepared:
                                   thisTaskType = EnumTaskType.VisaDocSubmission;
                                   AssignedToId = commondata.VisaProcessInchargeEmpId;
                                   candidateStatus="Under process - visa docs prepared";
                                   break;
                              /*
                              case EnumDeployStatus.VisaDocsSubmitted:
                                   thisTaskType = EnumTaskType.VisaReceived;
                                   AssignedToId = commondata.EmigProcessInchargeId;
                                   break;

                              case EnumDeployStatus.VisaQueryRaised:
                                   thisTaskType = EnumTaskType.VisaReceived;
                                   AssignedToId = commondata.EmigProcessInchargeId;
                                   break;
                              */
                              case EnumDeployStatus.VisaReceived:
                                   thisTaskType = EnumTaskType.EmigrationAppLodging;
                                   AssignedToId = commondata.TravelProcessInchargeId;
                                   candidateStatus="Under process - visa endorsed";
                                   break;
                              /*
                              case EnumDeployStatus.VisaDenied:
                                   thisTaskType = EnumTaskType.None;
                                   break;
                              */
                              case EnumDeployStatus.EmigDocsLodgedOnLine:
                                   thisTaskType = EnumTaskType.EmigrationDocsSubmitted;
                                   candidateStatus="Under process - emigration request submitted";
                                   AssignedToId = commondata.TravelProcessInchargeId;
                                   break;
                              /*
                              case EnumDeployStatus.EmigDocsPPSubmitted:
                                   thisTaskType = EnumTaskType.EmigrationGranted;

                                   break;

                              case EnumDeployStatus.EmigDocsQueryRecd:
                                   thisTaskType = EnumTaskType.EmigrationDocsSubmitted;
                                   break;

                              case EnumDeployStatus.EmigrationDenied:
                                   thisTaskType = EnumTaskType.None;
                                   break;
                              */
                              case EnumDeployStatus.EmigrationGranted:
                                   thisTaskType = EnumTaskType.TravelTicketBooking;
                                   candidateStatus="Under process - emigration clearance granted";
                                   break;
                              
                              case EnumDeployStatus.TravelTicketBooked:
                                   thisTaskType = EnumTaskType.TravelTicketBooking;
                                   candidateStatus="Under process - tickets booked";
                                   break;

                              case EnumDeployStatus.Traveled:
                                   thisTaskType = EnumTaskType.Traveled;
                                   candidateStatus="Traveled";
                                   break;
                              /*
                              case EnumDeployStatus.TravelCanceled:
                                   thisTaskType = EnumTaskType.None;
                                   break;
                              */
                              case EnumDeployStatus.ArrivalAcknowledgedByClient:
                                   thisTaskType = EnumTaskType.None;
                                   break;
                              default:
                                   break;
                         }
                    
                         //update Candidate.CandidateStatus
                         var cand = await _context.Candidates.FindAsync(cvreview.CandidateId);
                         cand.CandidateStatus=candidateStatus;
                         _unitOfWork.Repository<Candidate>().Update(cand);
                         
                         if(AssignedToId !=0)
                         {
                              var task = new ApplicationTask((int)thisTaskType, dt, loggedInEmployeeId, AssignedToId,
                                   commondata.OrderId, commondata.OrderNo, commondata.OrderItemId, "Task for you to organize " +
                                   ProcessName(deploy.NextStageId) + " for " + commondata.CandidateDesc, deploy.NextEstimatedStageDate,
                                   "Open", commondata.CandidateId, cvreview.CVReviewId);
                              _unitOfWork.Repository<ApplicationTask>().Add(task);
                         }
               }
               // A - 
               if (await _unitOfWork.Complete() > 0) return _mapper.Map<ICollection<Deploy>, ICollection<DeployAddedDto>>(deploys);

               return null;
          }
         
          public async Task<bool> DeleteDeploymentTransactions(Deploy deploy)
          {
               var deployToDelete = await _context.Deploys.FindAsync(deploy.Id);
               var dep = await _context.Deploys
                    .Where(x => x.CVRefId == deploy.CVRefId && x.Id != deploy.Id)
                    .OrderByDescending(x => x.TransactionDate)
                    .Select(x => new { x.StageId, x.TransactionDate }).Take(1).FirstOrDefaultAsync();

               _unitOfWork.Repository<Deploy>().Delete(deployToDelete);

               await _unitOfWork.Complete();

               return await UpdateCVRefWithDeployLastRecord(deploy.CVRefId);
          }

         
          public async Task<bool> EditDeploymentTransaction(Deploy deploy)
          {
               _unitOfWork.Repository<Deploy>().Update(deploy);
               await _unitOfWork.Complete();

               return await UpdateCVRefWithDeployLastRecord(deploy.CVRefId);

          }

          public async Task<CVRef> GetDeploymentsByCandidateAndOrderItem(int candidateId, int orderItemId)
          {
               return await _context.CVRefs.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
               .Include(x => x.Deploys.OrderByDescending(x => x.TransactionDate)).FirstOrDefaultAsync();
          }

          public async Task<CVRef> GetDeploymentsById(int cvrefid)
          {
               return await _context.CVRefs.Where(x => x.Id == cvrefid)
               .Include(x => x.Deploys.OrderByDescending(x => x.TransactionDate)).FirstOrDefaultAsync();
          }

          public async Task<CVReferredDto> GetDeploymentDto(int cvrefid)
          {
                var qry = (from r in _context.CVRefs where r.Id == cvrefid
                    join c in _context.Candidates on r.CandidateId equals c.Id
                    join i in _context.OrderItems on r.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    select new CVReferredDto {
                         CvRefId = cvrefid, CustomerName = o.Customer.CustomerName,
                         OrderId = o.Id, OrderDate = o.OrderDate, OrderItemId = i.Id,
                         CategoryName = i.Category.Name, CategoryRef = o.OrderNo + "-" + i.SrNo,
                         CustomerId = o.CustomerId, CandidateId = r.CandidateId, 
                         ApplicationNo = r.Candidate.ApplicationNo, CandidateName = r.Candidate.FullName,
                         ReferredOn = r.ReferredOn, SelectedOn = r.RefStatusDate,
                    })
                    .AsQueryable();
               var cvref = await qry.FirstOrDefaultAsync();
               var dep = await _context.Deploys.Where(x => x.CVRefId==cvrefid)
                    .OrderByDescending(x => x.TransactionDate).ToListAsync();

               var statuses = await _context.DeployStatus.OrderBy(x => x.StageId).Select(x => new {StageId = x.StageId, StatusName = x.StatusName}).OrderBy(x => x.StageId).ToListAsync();

               var dtos = new List<DeployRefDto>();
               foreach(var d in dep)
               {
                    dtos.Add(new DeployRefDto {CvRefId = d.CVRefId,TransactionDate = d.TransactionDate, 
                         DeploymentStatusname = statuses.Find(x => x.StageId==(int)d.StageId).StatusName});
               }
               
               var dto = new CVReferredDto();
               dto = cvref;
               dto.Deployments = dtos;
               return dto;
               
               /*
               var q = await _context.CVRefs.Where(x => x.Id == cvrefid)
                    .Include(x => x.OrderItem).ThenInclude(y => y.Category)
                    .Include(z => z.Candidate)
                    .FirstOrDefaultAsync();
                    
               return q;
               */
               //return await qry.FirstOrDefaultAsync();
                    
          }
          private async Task<Deploy> VerifyModelStageInSeqAndUpdateNextStage(Deploy deploy)
          {
               //return deploy model if in sequence, else return null;
               var lastStatusStageId = await _context.Deploys.Where(x => x.CVRefId == deploy.CVRefId)
               .MaxAsync(x => (int?)x.StageId) ?? 0;
               //var nextQuestionNo = await _context.AssessmentQsBank.MaxAsync(x => (int?)x.QNo) ?? 1;

               if ((EnumDeployStatus)lastStatusStageId == EnumDeployStatus.Concluded) return null;

               //  todo - return null if flg is out of sequence
               bool statusNotRelevantToProcess = false;
               EnumDeployStatus iStageId=EnumDeployStatus.None;
               EnumDeployStatus iNextStageId=EnumDeployStatus.None;
               switch (deploy.StageId)
               {
                    //following actions are not relevant to the process, hence can be out of sequence

                    case EnumDeployStatus.OfferLetterAccepted:
                         deploy.NextStageId = EnumDeployStatus.ReferredForMedical;
                         statusNotRelevantToProcess = true;
                         break;
                    case EnumDeployStatus.VisaQueryRaised:
                         deploy.NextStageId = EnumDeployStatus.VisaReceived;
                         statusNotRelevantToProcess = true;
                         break;
                    case EnumDeployStatus.EmigDocsQueryRecd:
                         deploy.NextStageId = EnumDeployStatus.EmigrationGranted;
                         statusNotRelevantToProcess = true;
                         break;
                    default:
                         break;
               }
               if (statusNotRelevantToProcess) return deploy;

               // C 
               //status is relevant to process
               //if status id is blank, offer next logical stages
               var dep = await _context.Deploys.Where(x => x.CVRefId == deploy.CVRefId)
                   .OrderByDescending(x => x.StageId).Take(1).FirstOrDefaultAsync();           //latest record
               if (dep == null)              //no record, so likely it is first entry after selection
               {
                    if (deploy.StageId != EnumDeployStatus.Selected)
                    {
                         return null;
                    }
               }
               else
               {
                    //if model does not have a stageId defined, then define one
                    iStageId = deploy.StageId == EnumDeployStatus.None 
                         ? await _context.Deploys.Where(x => x.CVRefId == dep.CVRefId)
                              .OrderByDescending(x => x.StageId).Select(x => x.StageId).FirstOrDefaultAsync()
                         : deploy.StageId;
                    deploy.StageId = iStageId;

                    var nextStage = await _context.DeployStatus.Where(x => (EnumDeployStatus)x.StageId == iStageId)
                         .Select(x => new { x.NextStageId, x.WorkingDaysReqdForNextStage }).FirstOrDefaultAsync();

                    if (nextStage == null) return null;
                    
                    if (iNextStageId != EnumDeployStatus.Concluded)
                    {
                         var isCandidateEcnr = false;
                         //TODO - add working days instead of days
                         deploy.StageId = iStageId;
                         if (deploy.StageId == EnumDeployStatus.VisaReceived)
                         {
                              isCandidateEcnr = await _context.CVRefs.Where(x => x.Id == dep.CVRefId)
                                   .Select(x => x.Ecnr).FirstOrDefaultAsync();
                         }
                         deploy.NextStageId = (isCandidateEcnr && iStageId == EnumDeployStatus.VisaReceived)
                              ? EnumDeployStatus.TravelTicketBooked : (EnumDeployStatus)nextStage.NextStageId;

                         deploy.NextEstimatedStageDate = deploy.TransactionDate.AddDays(nextStage.WorkingDaysReqdForNextStage);
                    }
               }
               return deploy;
          }
          
          private async Task<string> ProcessName(EnumDeployStatus ProcessId)
          {
               return await _context.DeployStatus.Where(x => (EnumDeployStatus)x.Id == ProcessId).Select(x => x.ProcessName).FirstOrDefaultAsync();
          }

          private async Task<bool> UpdateCVRefWithDeployLastRecord(int cvrefid)
          {
               var deployLastRecord = await _context.Deploys.Where(x => x.CVRefId == cvrefid)
               .OrderByDescending(x => x.TransactionDate).Take(1).FirstOrDefaultAsync();

               var cvrefToUpdate = await _context.CVRefs.FindAsync(cvrefid);
               cvrefToUpdate.DeployStageId = (int)deployLastRecord.StageId;
               cvrefToUpdate.DeployStageDate = deployLastRecord.TransactionDate;

               _unitOfWork.Repository<CVRef>().Update(cvrefToUpdate);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<DeployStatusDto>> GetDeployStatuses()
          {
               var lst = await _context.DeployStatus.OrderBy(x => x.StageId).Select(x => new DeployStatusDto{StageId=x.StageId, StatusName=x.StatusName}).ToListAsync();
               return lst;
          }
     }
}