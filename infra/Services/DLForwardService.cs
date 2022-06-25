using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Orders;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;

namespace infra.Services
{
     public class DLForwardService : IDLForwardService
     {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ATSContext _context;
        private readonly ICommonServices _commonService;
          private readonly IComposeMessagesForHR _composeMsgHR;
        public DLForwardService(IUnitOfWork unitOfWork, ATSContext context, ICommonServices commonService, IComposeMessagesForHR composeMsgHR)
        {
            _composeMsgHR = composeMsgHR;
            _context = context;
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        public async Task<bool> ForwardDLToAgents(OrderItemsAndAgentsToFwdDto itemsAndAgents, int LoggedInUserId)
        {
            var dlForwards = new List<DLForward>();
            foreach(var itemid in itemsAndAgents.Items) {
                var CatRef = await _commonService.CategoryRefFromOrderItemId(itemid.OrderItemId);
                foreach(var id in itemsAndAgents.Agents) {
                    if (string.IsNullOrEmpty(id.OfficialEmailId) && string.IsNullOrEmpty(id.Mobile) && string.IsNullOrEmpty(id.Phoneno)) break;
                    
                    var dlForward = new DLForward(itemid.OrderItemId, itemid.CategoryId, CatRef, id.OfficialId, 
                        itemsAndAgents.DateForwarded, id.OfficialEmailId, id.Phoneno, id.Mobile, LoggedInUserId);
                    dlForwards.Add(dlForward);
                    _unitOfWork.Repository<DLForward>().Add(dlForward);
                }
            }
            if (dlForwards.Count == 0) throw new Exception ("either email Id, Phone No or WhatsAppNo of the associates must be provided");
            var affected = await _unitOfWork.Complete();
            if (affected == 0) return false;

            var msgs = await _composeMsgHR.ComposeHTMLToForwardEnquiryToAgents(itemsAndAgents, LoggedInUserId);
            
            if (msgs.EmailMessages != null & msgs.EmailMessages.Count() > 0 ) {
                foreach(var msg in msgs.EmailMessages) {
                    _unitOfWork.Repository<EmailMessage>().Add(msg);
                }
                if (await _unitOfWork.Complete() > 0) {
                    //send email messages
                } else {
                    throw new Exception("failed to send email messages");
                }
            }
            if (msgs.SMSMessages != null & msgs.SMSMessages?.Count() > 0 ) {
                foreach(var msg in msgs.SMSMessages) {
                    _unitOfWork.Repository<SMSMessage>().Add(msg);
                }
                if (await _unitOfWork.Complete() > 0) {
                    //send SMS messages
                } else {
                    throw new Exception("failed to send SMS Messages");
                }
            }
            if (msgs.WhatsAppMessages != null & msgs.WhatsAppMessages?.Count() > 0 ) {
                foreach(var msg in msgs.WhatsAppMessages) {
                    _unitOfWork.Repository<SMSMessage>().Add(msg);
                }
                if (await _unitOfWork.Complete() > 0) {
                    //send WhatsApp messages
                } else {
                    throw new Exception ("failed to send WhatsApp messages");
                }
            }
            
            return true;
        }
    }
}