using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace core.Entities.Admin
{
    public enum EnumCustomerStatus
    {
        [Display(Name = "Active"), EnumMember(Value="Active")] Active = 100,
        [Display(Name = "PClosed"), EnumMember(Value="Closed")] PhoneNotReachable = 200,
        [Display(Name = "Blacaklisted"), EnumMember(Value="Blacklisted")] Blacklisted = 300
    }
}