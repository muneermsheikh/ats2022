using System.Runtime.Serialization;

namespace core.Entities.MasterEntities
{
    public class ReviewStatus: BaseEntity
    {
       public string Status { get; set; }
    }

    public enum EnumReviewStatus {
        [EnumMember(Value="Not Reviewed")]
        NotReviewed,
        [EnumMember(Value="Regretted")]
        Regretted,
        [EnumMember(Value="Accepted with some regrets")]
        AcceptedWithSomeRegrets,
        [EnumMember(Value="Accepted")]
        Accepted
    }
}