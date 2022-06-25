using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
    public class ComposeMessagesForProcessing : IComposeMessagesForProcessing
    {
        private readonly ATSContext _context;
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _confg;
          private readonly IEmployeeService _empService;
        public ComposeMessagesForProcessing(ATSContext context, ICommonServices commonServices, IConfiguration confg, IEmployeeService empService)
        {
               _empService = empService;
               _confg = confg;
               _commonServices = commonServices;
               _context = context;
        }

        public async Task<EmailMessage> AdviseProcessTransactionUpdatesToCandidateByEmail(DeployMessageParamDto deployParam)
        {
            var deploy = deployParam.Deploy;
            var candidate = await (from cvref in _context.CVRefs
                                where cvref.Id == deploy.CVRefId
                                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                                select cand).FirstOrDefaultAsync();

            var candidateName = candidate.FullName;
            var email = candidate.Email;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";
            var subject = "Your Process Transaction Update";
            var msg = "";
            msg = title + " " + candidateName + "<br>Application No.:" + candidate.ApplicationNo + ", PP No.:" + candidate.PpNo;
            msg += "email: " + candidate.Email + "<br><br>Dear Sir:<br><br>please be advised of the following updates to your departure formalities<br><br>";
            msg += "<tab>Date: " + deploy.TransactionDate.Date;
            msg += "<br><tab>Transaction: " + _commonServices.DeploymentStageNameFromStageId((int)deploy.StageId);
            msg += deploy.NextStageId > 0 ? "<br><tab>Next Stage:" + _commonServices.DeploymentStageNameFromStageId((int)deploy.NextStageId) + ", scheduled by " + deploy.NextEstimatedStageDate.Date : "";
            msg += "<br><br>Regards/Processing Divn";

            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = _confg["EmailSenderEmailId"],
                SenderUserName = _confg["EmailSenderDisplayName"],
                RecipientUserName = candidate.KnownAs,
                RecipientEmailAddress = email,
                CcEmailAddress = _confg["EmailCCandAck"],
                BccEmailAddress = _confg["EmailBCCandAck"],
                Subject = subject,
                Content = msg,
                MessageTypeId = (int)EnumMessageType.MedicalExaminationAdvisebyEmail       //todo - change to individual process types
            };

            return emailMessage;
        }

        public async Task<SMSMessage> AdviseProcessTransactionUpdatesToCandidateBySMS(DeployMessageParamDto deployParam)
        {
            var deploy = deployParam.Deploy;
            var candidate = await (from cvref in _context.CVRefs
                                where cvref.Id == deploy.CVRefId
                                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                                select cand).FirstOrDefaultAsync();

            var candidateName = candidate.KnownAs;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            var msg = "";

            var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
            if (string.IsNullOrEmpty(mobileNo)) return null;

            msg = title + " " + candidateName + ", PP No.:" + candidate.PpNo;
            msg += "pl be advised of flg updates to yr departure formalities<br><br>";
            msg += "<tab>Date: " + deploy.TransactionDate.Date;
            msg += "<br><tab>Transaction: " + _commonServices.DeploymentStageNameFromStageId((int)deploy.StageId);
            msg += deploy.NextStageId > 0 ? "<br><tab>Next Stage:" + _commonServices.DeploymentStageNameFromStageId((int)deploy.NextStageId) + ", scheduled by " + deploy.NextEstimatedStageDate.Date : "";
            msg += "<br><br>Regards/Processing Divn";

            var smsMessage = new SMSMessage
            {
                UserId = candidate.AppUserId,
                PhoneNo = mobileNo,
                SMSText = msg
            };

            return smsMessage;
        }

        public async Task<EmailMessage> ComposeAppplicationTaskMessage (int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null) return null;

            var email = new EmailMessage();

            var addressee = await _empService.GetEmployeeBriefAsyncFromEmployeeId(task.AssignedToId);
            var sender = await _empService.GetEmployeeBriefAsyncFromEmployeeId(task.TaskOwnerId);

            var mailBody = DateTime.Now.Date + "<br><br>" + addressee.Gender == "M" ? "Mr. " : "Ms, " + addressee.EmployeeName + "<br>";
            mailBody += "Target Date: " + task.CompleteBy.Date + "<br>";
            mailBody += string.IsNullOrEmpty(addressee.Position)==true ? "" : addressee.Position + "<br>";
            mailBody += "Email: " +addressee.OfficialEmailAddress + "<br><br>Following task is assigned to you:<br><br>";
            mailBody += "Date of the task: " + task.TaskDate.Date + "<br>";
            mailBody += "Assigned By: " + sender.EmployeeName;
            mailBody += string.IsNullOrEmpty(sender.Position)==true ? "" : ", " + ", Position:" + sender.Position;
            mailBody += "<br>Phone No.: " + sender.OfficialPhoneNo;
            mailBody += "<br><br><b>Task Description</b><br>:" + task.TaskDescription;

            email.Content=mailBody;
            email.RecipientEmailAddress = addressee.OfficialEmailAddress;
            email.MessageTypeId = task.TaskTypeId;
            email.RecipientUserName = addressee.UserName;
            email.RecipientId = addressee.AppUserId;
            email.SenderEmailAddress = sender.OfficialEmailAddress;
            email.SenderId = sender.AppUserId;
            email.SenderUserName = sender.UserName;
            email.Subject = task.Id + " dated " + task.TaskDate.Date;

            return email;
        }

    }
}