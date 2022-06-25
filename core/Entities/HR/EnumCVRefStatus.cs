using System.Runtime.Serialization;

namespace core.Entities.HR
{
    public enum EnumCVRefStatus
    {
        [EnumMember(Value="Not Referred")] NotReferred=0,
        [EnumMember(Value="Referred")] Referred=11,
        [EnumMember(Value="Selected")] Selected=10,
        [EnumMember(Value="Rejected - Not Suitable")] RejectedNotSuitable=300,
        [EnumMember(Value="Rejected - Medically Unfit")] RejectedMedicallyUnfit=400,
        [EnumMember(Value="Rejected - Salary Expectation High")] RejectedHighSalaryExpectation=3,
        [EnumMember(Value="Rejected - No relevant exp")] RejectedNoRelevantExp=6,
        [EnumMember(Value="Rejected - Not qualified")] RejectedNotQualified=12,
        [EnumMember(Value="Rejected - Overage")] RejectedOverAge=2,
        [EnumMember(Value="Rejected - Not Available")] NotAvailable=900,
        [EnumMember(Value="Not Interested")] NotInterested=13
    }
}