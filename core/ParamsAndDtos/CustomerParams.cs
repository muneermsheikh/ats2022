
using System;
using core.Entities.Admin;
using core.Params;

namespace core.ParamsAndDtos
{
    public class CustomerParams: ParamPages
    {
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OfficialName {get; set;}
        public string phoneNo {get; set;}
        public string mobileNo {get; set;}
        public EnumCustomerStatus CustomerStatus {get; set;}
        public int IndustryId {get; set;}
        public string City {get; set;}
        public string Country {get; set;}
        
    }
}