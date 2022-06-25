using System.Runtime.Serialization;

namespace core.Entities.Orders
{
    public enum EnumOrderItemStatus
    {
        [EnumMember(Value="Not Started")]
        NotStarted = 0,
        [EnumMember(Value="UnderProcess")]
        UnderProcess=100,
        [EnumMember(Value="Canceled - client not serious")]
        Canceled_ClientNotSerious=200,
        [EnumMember(Value="Canceled - Unable to supply")]
        Canceled_UnableToSupply=300,
        [EnumMember(Value="Canceled - Inactive Category")]
        Canceled_InactiveCategory=400,
        [EnumMember(Value="Concluded")]
        Concluded=500,
        [EnumMember(Value="ConcludedPartially")]
        ConcludedPartially=600
        
    }
}