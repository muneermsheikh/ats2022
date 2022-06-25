using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IComposeMessagesForProcessing
    {
         //process        
        Task<EmailMessage> AdviseProcessTransactionUpdatesToCandidateByEmail(DeployMessageParamDto deploy);
        Task<SMSMessage> AdviseProcessTransactionUpdatesToCandidateBySMS(DeployMessageParamDto deploy);
        
        Task<EmailMessage> ComposeAppplicationTaskMessage (int taskId);

    }
}