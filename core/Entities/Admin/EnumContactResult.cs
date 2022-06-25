using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace core.Entities.Admin
{
    public enum EnumContactResult
    {
        //100 to 1000 - connectivity problems
        [Display(Name = "Phone Not Reachable."), EnumMember(Value="Phone Not Reachable")] PhoneNotReachable = 1,
        [Display(Name = "Unable to connect video conference application."), EnumMember(Value="Unable to connect Video Conference Application")] VideoConfToolUnableToConnect = 2,
        [Display(Name = "Offer Accepted."), EnumMember(Value="Offer Accepted")] OfferAccepted = 3,
        [Display(Name = "Interested in Offer."), EnumMember(Value="Interested In offer")] Interested = 4,
        [Display(Name = "Wrong phone number."), EnumMember(Value="Phone Wrong number")] WrongNumber = 5,
        [Display(Name = "Not Interested due to Unknown REasons."), EnumMember(Value="Phone Wrong number")] NotItnerestedUnknownReasons = 6,
        [Display(Name = "Service Charges Not Acceptable."), EnumMember(Value="Phone Wrong number")] ServiceChargesNotAcceptable = 7,
        [Display(Name = "Does not possess Passport."), EnumMember(Value="Phone Wrong number")] DoesNotHavePP = 8,
        [Display(Name = "Salary Not Acceptable."), EnumMember(Value="Phone Wrong number")] SalaryNotAcceptable = 9,
        [Display(Name = "Does Not Sound Interested."), EnumMember(Value="Phone Wrong number")] DoesNotSoundInterested = 10,
        [Display(Name = "Rude  Behavior."), EnumMember(Value="Phone Wrong number")] RudeBehavior = 11,
        [Display(Name = "Not interested in the job."), EnumMember(Value="Not Interested in the Job")] NotInterested = 12,
        [Display(Name = "PHone answered by someone else."), EnumMember(Value="Phone answered by someone else")] PhoneAnsweredBySomeoneElse = 14,
        [Display(Name = "Phone unanswered."), EnumMember(Value="Phone unanswered")] PhoneUnanswered = 15,
        [Display(Name = "Phone out of network."), EnumMember(Value="Phone Out of network")] PhoneOutOfNetwork = 16,
        [Display(Name = "Wants salary increased."), EnumMember(Value="Wants salary increased")] SalaryRangeNotAcceptable = 17
    }       
}