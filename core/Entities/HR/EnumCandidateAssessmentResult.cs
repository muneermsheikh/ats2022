using System.Runtime.Serialization;

namespace core.Entities.HR
{
    public enum EnumCandidateAssessmentResult
    {
        [EnumMember(Value="Grade A+ - Excellent - 80%+")]
        Excellent=80,
        [EnumMember(Value="Grade A - Very Good - 70%+")]
        VeryGood=70,
        [EnumMember(Value="Grade B - Good - 60%+")]
        Good=60,
        [EnumMember(Value="Grade C - Poor - 50%+")]
        Poor=50,
        [EnumMember(Value="Grade D - Very Poor - 49%-")]
        VeryPoor=49,
        [EnumMember(Value ="Not Assessed")]
        NotAssessed=0,
        [EnumMember(Value ="Not Required")] NotRequired=-1
    }
}