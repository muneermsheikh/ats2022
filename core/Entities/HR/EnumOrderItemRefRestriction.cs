using System.Runtime.Serialization;

namespace core.Entities.HR
{
    public enum EnumOrderItemRefRestriction
    {
        [EnumMember(Value="CV Ref exceed minimum count")]
        CVRefExceedCount,
        [EnumMember(Value="Job aborted")]
        JobAborted,
        [EnumMember(Value="Canceled by Customer")]
        CanceledByCustomer

    }
}