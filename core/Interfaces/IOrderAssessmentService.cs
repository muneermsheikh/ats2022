using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IOrderAssessmentService
    {
        //stddASsessment
        Task<ICollection<OrderItemAssessment>> CreateNewOrderAssessment(int orderid);
        Task<OrderItemAssessment> CopyStddQToOrderAssessmentItem(int orderitemid);
        Task<OrderItemAssessment> AddNewOrderAssessmentItem(int orderitemid);
        Task<IReadOnlyList<AssessmentQBank>> GetAssessmentQsFromBankBySubject (AssessmentStddQsParams qsParams);
        
        //orderAssessment
        Task<OrderItemAssessment> GetOrAddOrderAssessmentItem (int OrderItemId);
        Task<ICollection<OrderItemAssessment>> GetOrderAssessment (int orderId);
        Task<bool> EditOrderAssessmentItem(OrderItemAssessment assessmentItem);
        Task<bool> EditOrderAssessmentQs(ICollection<OrderItemAssessmentQ> assessmentQs);
        Task<bool> EditOrderAssessmentQ(OrderItemAssessmentQ assessmentQ);
        Task<bool> DeleteAssessmentItemQ(int orderitemid);
        Task<bool> DeleteAssessmentQ(int assessmentQid);

        //assessmentQs
        Task<ICollection<OrderItemAssessmentQ>> GetAssessmentQsOfOrderItemId(int orderitemid);
        
    }
}