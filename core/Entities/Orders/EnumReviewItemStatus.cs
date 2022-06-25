using System;
using System.Runtime.Serialization;

namespace core.Entities.Orders
{
    public enum EnumReviewItemStatus
    {
        [EnumMember(Value="Not Reviewed")]
        NotReviewed = 0, 
        [EnumMember(Value="Accepted")]
        Accepted=100,
        [EnumMember(Value="Declined - Salary Not Feasible")]
        SalaryNotFeasible=200,
        [EnumMember(Value="Declined - Visa availability uncertain")]
        VisaAvailabilityUncertain=300,
        [EnumMember(Value="Declined - Commercially Not Feasible")]
        CommerciallyNotFeasible=400,
        [EnumMember(Value="Declined - Logistically Not Feasible")]
        LogisticallyNotFeasible=500,
        [EnumMember(Value="Negative Background report on customer")]
        NotAccepted=600,
        [EnumMember(Value="Not Accepted")]
        NegativeBackgroundReport=1000

    }
}