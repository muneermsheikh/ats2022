using System.Runtime.Serialization;

namespace core.Entities.Users
{
    public enum EnumCandidateStatus
    {
        [EnumMember(Value="Not Referred")]
        NotReferred = 100, 
        [EnumMember(Value="Referred, awaiting selection")]
        Referred = 200,
        [EnumMember(Value="Selected, deployment in process")]
        Selected = 300,
        [EnumMember(Value="Traveled, currently overseas")]
        Traveled = 400,
        [EnumMember(Value="Not Available")]
        NotAvailable = 500
        
    }
}