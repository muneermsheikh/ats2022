using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface ISelectionDecisionService
    {
        Task<SelectionMsgsAndEmploymentsDto> RegisterSelections(ICollection<SelDecisionToAddDto> selDto, int loggedInEmpId);
         Task<bool> EditSelection(SelectionDecision selectionDecision);
         Task<bool> DeleteSelection(int id);
         Task<Pagination<SelectionDecision>> GetSelectionDecisions (SelDecisionSpecParams specParams);
         Task<Pagination<SelectionsPendingDto>> GetPendingSelections(CVRefSpecParams refParams);
         Task<ICollection<SelectionStatus>> GetSelectionStatus();
         
    }
}