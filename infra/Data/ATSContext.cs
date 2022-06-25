using System.Reflection;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Attachments;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Entities.Messages;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Entities.Users;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace infra.Data
{
    public class ATSContext: DbContext
    {
          public ATSContext(DbContextOptions<ATSContext> options) : base(options)
          {
          }

          protected ATSContext()
          {
          }

          public DbSet<FileOnFileSystem> FilesOnFileSystem {get; set;}
          //general
          public DbSet<Customer> Customers {get; set;}
          public DbSet<CustomerIndustry> CustomerIndustries {get; set;}
          public DbSet<CustomerOfficial> CustomerOfficials {get; set;}
          public DbSet<AgencySpecialty> AgencySpecialties {get; set;}
          public DbSet<CustomerReview> CustomerReviews {get; set;}
          public DbSet<CustomerReviewItem> CustomerReviewItems {get; set;}
          public DbSet<CustomerReviewData> CustomerReviewDatas {get; set;}

     //HR
          public DbSet<ChecklistHR> ChecklistHRs {get; set;}
          public DbSet<CandidateAssessment> CandidateAssessments {get; set;}
          public DbSet<CandidateAssessmentItem> CandidateAssessmentItems {get; set;}
          public DbSet<ChecklistHRItem> ChecklistHRItems {get; set;}
          public DbSet<CVRef> CVRefs {get; set;}
          public DbSet<CVRefRestriction> CVRefRestrictions {get; set;}
          public DbSet<SelectionDecision> SelectionDecisions {get; set;}
          public DbSet<Employment> Employments {get; set;}
          public DbSet<CVRvw> CVReviews {get; set;}
          public DbSet<FileUpload> FileUploads {get; set;}
          public DbSet<Interview> Interviews {get; set;}
          public DbSet<InterviewItem> InterviewItems {get; set;}
          public DbSet<InterviewItemCandidate> InterviewItemCandidates {get; set;}
          public DbSet<InterviewItemCandidateFollowup> InterviewItemCandidatesFollowup {get; set;}

          //Not mapped
          //public DbSet<CandidateBriefDto> CandidateDto {get; set;}

     //Admin
          public DbSet<UserHistory> UserHistories {get; set;}
          public DbSet<UserHistoryItem> UserHistoryItems {get; set;}


       // masters
          //public DbSet<core.Entities.Users.Address> Addresses {get; set;}
          public DbSet<AssessmentQBank> AssessmentQBank { get; set; }
          public DbSet<AssessmentQBankItem> AssessmentQBankItems {get; set;}
          public DbSet<AssessmentStandardQ> AssessmentStandardQs {get; set;}
          public DbSet<Category> Categories {get; set;}
          public DbSet<ChecklistHRData> ChecklistHRDatas {get; set;}
          //public DbSet<DeployStage> DeployStages {get; set;}
          public DbSet<Employee> Employees {get; set;}
          public DbSet<EmployeeAddress> EmployeeAddresses {get; set;}
          public DbSet<EmployeeQualification> EmployeeQualifications {get; set;}
          public DbSet<EmployeeHRSkill> EmployeeHRSkills {get; set;}
          public DbSet<EmployeeOtherSkill> EmployeeOtherSkills {get; set;}
          public DbSet<EmployeePhone> EmployeePhones {get; set;}
          public DbSet<Industry> Industries {get; set;}
          public DbSet<JobDescription> JobDescriptions {get; set;}
          public DbSet<Qualification> Qualifications {get; set;}
          public DbSet<Remuneration> Remunerations {get; set;}
          public DbSet<ReviewItemStatus> ReviewItemStatuses {get; set;}
          public DbSet<ReviewStatus> ReviewStatuses {get; set;}
          public DbSet<SkillData> SkillDatas {get; set;}
          public DbSet<SelectionStatus> SelectionStatuses {get; set;}
          public DbSet<InterviewAttendanceStatus> InterviewAttendancesStatus {get; set;}
          public DbSet<ContactResult> ContactResults {get; set;}
          
     ///orders
          
          public DbSet<Order> Orders {get; set;}
          public DbSet<OrderItem> OrderItems {get; set;}
          public DbSet<ContractReview> ContractReviews {get; set;}
          public DbSet<ContractReviewItem> ContractReviewItems {get; set;}
          public DbSet<ReviewItem> ReviewItems {get; set;}
          public DbSet<ReviewItemData> ReviewItemDatas{get; set;}
          public DbSet<OrderItemAssessment> OrderItemAssessments {get; set;}
          public DbSet<OrderItemAssessmentQ> OrderItemAssessmentQs {get; set;}
          public DbSet<DLForward> DLForwards {get; set;}

      //Process
          public DbSet<Deploy> Deploys {get; set;}
          public DbSet<DeployStatus> DeployStatus {get; set;}
          
     //Tasks
          public DbSet<ApplicationTask> Tasks {get; set;}
          public DbSet<TaskItem> TaskItems {get; set;}
          public DbSet<TaskType> TaskTypes {get; set;}

     //users

          public DbSet<Candidate> Candidates {get; set;}
          public DbSet<ProspectiveCandidate> ProspectiveCandidates {get; set;}
          public DbSet<EntityAddress> EntityAddresses {get; set;}
          public DbSet<Photo> Photos {get; set;}
          //public DbSet<core.Entities.Users.Address> UserAddresses {get; set;}
          public DbSet<UserExp> UserExps {get; set;}
          //public DbSet<UserLike> UserLikes {get; set;}
          public DbSet<UserPassport> UserPassports {get; set;}
          public DbSet<UserPhone> UserPhones {get; set;}
          public DbSet<UserProfession> UserProfessions{get; set;}
          public DbSet<UserQualification> UserQualifications {get; set;}
          public DbSet<UserAttachment> UserAttachments {get; set;}
     // APPIDENTITYDBCONTEXT
          //public DbSet<UserLike> Likes { get; set; }
         public DbSet<Group> Groups { get; set; }
          public DbSet<Connection> Connections { get; set; }
    
    //    Messages
          public DbSet<MessageComposeSource> MessageComposeSources {get; set;}
          public DbSet<MessageType> MessageTypes {get; set;}
          public DbSet<EmailMessage> EmailMessages {get; set;}
          public DbSet<PhoneMessage> PhoneMessages {get; set;}
          public DbSet<SMSMessage> SMSMessages {get; set;}
     
     //View from SQLServer -
          //CVRefCount
          public DbSet<CVsRefCountDto> CVsRefCountDtos {get; set;}

          //required for migrations
          protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
               base.OnModelCreating(modelBuilder);
               modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
          }

    }
}