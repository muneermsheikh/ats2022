using System.Runtime.Serialization;

namespace core.Entities.Orders
{
    public enum EnumOrderStatus
    {
        [EnumMember(Value="Awaiting Review")]
        AwaitingReview = 0, 
        [EnumMember(Value="Reviewed and Accepted")]
        ReviewedAndAccepted = 100,
        [EnumMember(Value="Reviewed and declined")]
        ReviewedAndDeclined = 200,
        [EnumMember(Value="Assigned to HR")]
        AssignedToHR = 400,
        [EnumMember(Value="Concluded")]
        Concluded = 1000
        
    }
}