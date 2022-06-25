using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using AutoMapper;
using core.Entities.Admin;
using core.Entities.Attachments;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
     //[Authorize]
     public class CandidateController : BaseApiController
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IMapper _mapper;
          private readonly UserManager<AppUser> _userManager;
          private readonly SignInManager<AppUser> _signInManager;
          private readonly IUserService _userService;
          private readonly IGenericRepository<Candidate> _candRepo;
          private readonly IWebHostEnvironment _environment;
          private readonly IEmployeeService _empService;
          private readonly ILogger<CandidateController> _logger;
          const string FILE_PATH = @"D:\UploadedFiles\";
          public CandidateController(IUnitOfWork unitOfWork, IMapper mapper, IEmployeeService empService,
               IGenericRepository<Candidate> candRepo, ILogger<CandidateController> logger,
               UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment environment,
               IUserService userService)
          {
               _environment = environment;
               _candRepo = candRepo;
               _userService = userService;
               _signInManager = signInManager;
               _userManager = userManager;
               _mapper = mapper;
               _unitOfWork = unitOfWork;
               _empService = empService;
               _logger = logger;
          }

     
          [HttpGet("candidatepages")]
          public async Task<ActionResult<Pagination<CandidateBriefDto>>> GetCandidatePagesAsync([FromQuery]CandidateSpecParams candidateParam)
          {
               //if (!User.IsUserAuthenticated()) return Unauthorized("user is not authenticated");
               
               var cands = await _userService.GetCandidates(candidateParam);
               return cands;
          }

          
          [Authorize]
          [HttpGet("candidatelist")]
          public async Task<ActionResult<ICollection<CandidateBriefDto>>> GetCandidateListAsync(CandidateSpecParams candidateParam)
          {
               var spec = new CandidateSpecs(candidateParam);
               var countSpec = new CandidateForCountSpecs(candidateParam);

               var totalItems = await _unitOfWork.Repository<Candidate>().CountAsync(countSpec);
               if (totalItems == 0) return NotFound(new ApiResponse(404, "No records returned"));
               var cands = await _unitOfWork.Repository<Candidate>().ListAsync(spec);

               var dtos = new List<CandidateBriefDto>();
               foreach(var cand in cands)
               {
                    if (cand.UserProfessions != null && cand.UserProfessions.Count > 0) {
                         foreach(var prof in cand.UserProfessions) {
                              if (string.IsNullOrEmpty(prof.Profession)) prof.Profession = await _userService.GetCategoryNameFromCategoryId(prof.CategoryId);
                         }
                    }

                    dtos.Add(new CandidateBriefDto{Id = cand.Id, FullName = cand.FirstName + " " + cand.SecondName??"" + " " + cand.FamilyName??"", 
                         City=cand.City, ApplicationNo = cand.ApplicationNo, ReferredById = cand.ReferredBy,
                         ReferredByName= await _userService.GetCustomerNameFromCustomerId(cand.ReferredBy),
                         UserProfessions = cand.UserProfessions});
               }
               //var data = _mapper.Map<IReadOnlyList<CandidateToReturnDto>>(cands);
               
               return Ok(dtos);
               //return Ok(new Pagination<CandidateBriefDto>(candidateParam.PageIndex,
                    //candidateParam.PageSize, totalItems, cands));
          }

          [HttpGet("byid/{id}")]
          public async Task<ActionResult<Candidate>> GetCandidateById(int id)
          {
               var cand = await _userService.GetCandidateByIdWithAllIncludes(id);
               return Ok(cand);
          }

          [HttpGet("byappno/{appno}")]
          public async Task<ActionResult<CandidateBriefDto>> GetCandidateFromApplicationNo(int appno) {
               var cv = await _userService.GetCandidateByAppNo(appno);
               if (cv==null) return NotFound(new ApiResponse(404, "Application No. " + appno + " not found"));

               return Ok(cv);
          }
          
          [HttpGet("briefbyid/{id}")]
          public async Task<ActionResult<CandidateBriefDto>> GetCandidateBriefById(int id) {
               var cv = await _userService.GetCandidateBriefById(id);
               
               if (cv==null) return NotFound(new ApiResponse(404, "Candidate with id " + id + " not found"));

               return Ok(cv);
          }
          
          /*
          [HttpGet("candidatebyid/{userid}")]
          public async Task<ActionResult<Candidate>> GetCandidatebyUserId(int userid)
          {
               return await _userService.GetCandidateByIdAsync(userid);
          }

          [HttpGet("candidatebyappuserid/{appuserid}")]
          public async Task<ActionResult<Candidate>> GetCandidateByAppUserid(int appUserId)
          {
          var cands = await _userService.GetCandidateBySpecsIdentityIdAsync(appUserId);
          if (cands == null) return NotFound(new ApiResponse(404));
          return Ok(cands);
          }
          */
          
          [HttpGet("emailexists")]
          public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }

          [HttpGet("cities")]
          public async Task<ActionResult<ICollection<CandidateCity>>> GetCandidateCities()
          {
               var c = await _userService.GetCandidateCityNames();

               if (c.Count() == 0) return NotFound();
               return Ok(c);
          }
          
          
          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPost]
          public async Task<ActionResult<bool>> UploadUserAttachmentsR(FileToUpload file) {

               var filePathName = FILE_PATH + Path.GetFileNameWithoutExtension(file.FileName) + "-" +
                    DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                    Path.GetExtension(file.FileName);
               
               return Ok(true);

          }


          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPost("attachment/{candidateAppUserId}")]
          public async Task<ActionResult<bool>> UploadUserAttachments(ICollection<IFormFile> files, int candidateAppUserId)
          {
               if (files.Count() == 0) return BadRequest(new ApiResponse(404, "No files to attach"));
          
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               int loggedInEmployeeId = loggedInUser == null ? 0 : await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);

               string Errorstring = "";
               var filesUploaded = new List<FileUpload>();
               Errorstring = FileExtensionsOk(files);
               
               if (!string.IsNullOrEmpty(Errorstring))                     
                    return BadRequest(new ApiResponse(402, "file upload failed due to following error: " +
                         Environment.NewLine + Errorstring));

               try
               {
                    foreach (var file in files)
                    {
                         var folder = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files");
                         if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                         var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files", file.FileName);
                         if (!System.IO.File.Exists(path))
                         {
                              using(var stream = new FileStream(path, FileMode.Create))
                              {
                                   await file.CopyToAsync(stream);         //file coied to path
                              }
                              
                              var f = new FileUpload(candidateAppUserId, file.FileName.Substring(0,3),
                                   file.Length, User.GetUsername(), loggedInUser == null ? 0 : loggedInEmployeeId,
                                   DateTime.Now, true);
                              _unitOfWork.Repository<FileUpload>().Add(f);
                              filesUploaded.Add(f);
                         } else {
                              Errorstring +="file " + file.FileName + " already is uploaded";
                         }
                    }

                    if (filesUploaded.Count > 0) {
                         await _unitOfWork.Complete();
                         if(!string.IsNullOrEmpty(Errorstring)) {
                              return Ok(Errorstring + Environment.NewLine + "Other files uploaded successfully");
                         }  else {
                              return Ok();
                         }
                    } 
                    else {
                         return BadRequest(new ApiResponse(404,  "failed to upload" + Environment.NewLine + Errorstring));
                    }
               }
               catch (Exception ex)
               {
                    return BadRequest(new ApiResponse(402, ex.Message));
               }

          }

           [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive, Candidate")]
          [HttpDelete("deleteUploadedFile")]
          public async Task<ActionResult<bool>> DeleteUploadedFile(FileUpload fileupload)
          {
               var fileWithPathName = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files", fileupload.Name);
               try {
                    System.IO.File.Delete(fileWithPathName);
                    //update DB
                    _unitOfWork.Repository<FileUpload>().Delete(fileupload);
                    await _unitOfWork.Complete();
                    return Ok(true);
               } catch (Exception ex) {
                    return BadRequest(new ApiResponse(404, ex.Message));
               }
          }

          [HttpGet("downloadfile/{id:int}")]
          public async Task<ActionResult> DownloadFile(int id)
          {
               // ... code for validation and get the file
               
               var file = await _unitOfWork.Repository<FileUpload>().GetByIdAsync(id);
               if (file==null) return BadRequest(new ApiResponse(404, "No such file exists"));

               var filePath = Path.Combine(file.UploadedLocation + "\\" + file.Name);  
               // Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files", file.Name);
               
               if (!System.IO.File.Exists(filePath)) return BadRequest(new ApiResponse(404, "Though the file exists in database record, no physical file could be located"));

               var provider = new FileExtensionContentTypeProvider();
               if (!provider.TryGetContentType(filePath, out var contentType))
               {
                    contentType = "application/octet-stream";
               }
               
               var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
               return File(bytes, contentType, Path.GetFileName(filePath));
          }

          private string FileExtensionsOk(ICollection<IFormFile> formFiles)
          {
               var ok = true;
               string ext = "";
               string errorString="";
               var files = formFiles.Select(x => new {x.FileName, x.Length}).ToList();
               foreach(var file in files)
               {
                    var fileType = file.FileName.Trim().ToLower().Substring(0,3);
                    switch(fileType)
                    {
                         case "cv-":
                              ext = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                              ok =  (ext == ".docx" || ext == ".doc" || ext == ".pdf");
                              if (!ok) errorString += Environment.NewLine + "only docx, doc or pdf extensions acceptable for attachment type CV";
                              if(file.Length == 0 ||file.Length > 1024*1024*3) errorString +=Environment.NewLine + "File size cannot exceed 3MB";
                              break;
                         case "ec-":
                         case "qc-":
                         case "pp-":
                              ext = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                              ok =  (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".pdf");
                              if (!ok) errorString += Environment.NewLine + "only jpeg, jpg, png or pdf extensions acceptable for attachment type Certificates";
                              if(file.Length == 0 ||file.Length > 1024*1024*3) errorString +=Environment.NewLine + "File size cannot exceed 3MB";
                              break;
                         case "ph-":
                              ext = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                              ok =  (ext == ".jpeg" || ext == ".jpg" || ext == ".png");
                              if (!ok) errorString += Environment.NewLine + "only jpeg, jpg or png extensions acceptable for images";
                              if(file.Length == 0 ||file.Length > 1024*1024*3) errorString +=Environment.NewLine + "File size cannot exceed 3MB";
                              break;
                         default:
                              errorString += "prefix " + fileType + " not recognized";
                              break;
                    }
               }
               return errorString;
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpPut("edituserprof")]
          public async Task<ActionResult<UserAndProfessions>> EditUserProfessions(UserAndProfessions userProfessions)
          {
               //var lst = roles.Split(",").ToArray();
               var candidate = await _unitOfWork.Repository<Candidate>().GetByIdAsync(userProfessions.CandidateId);
               if (candidate == null) return BadRequest(new ApiResponse(404, "No such candidate on record"));
          
               /* 
                    var professions = await _unitOfWork.Repository<UserProfession>().ListAsync(
                    new UserProfessionsSpecs(new UserProfessionsSpecParams{CandidateId = candidateId}));
               */
               var professions = await _userService.EditUserProfessions(userProfessions);
               if (professions == null) return BadRequest(new ApiResponse(404, "Failed to edit the user professions"));
               var profs = _mapper.Map<List<UserProfession>, List<Prof>>(professions.ToList());
               
               return Ok(new UserAndProfessions{CandidateId = userProfessions.CandidateId, CandidateProfessions = profs});
          }
     
           [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRTrainee")]
          [HttpPut]
          public async Task<ActionResult<Candidate>> EditCandidate(Candidate candidate)
          {
               var cand = await _userService.UpdateCandidateAsync(candidate);
               if (cand == null) return BadRequest(new ApiResponse(404, "Failed to update the candidate"));
               return Ok(cand);
          }

         

    }


}