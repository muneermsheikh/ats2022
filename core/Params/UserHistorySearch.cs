using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Params
{
    public class UserHistorySearch
    {
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string AadharNo { get; set; }
        public int? ApplicationNo { get; set; }
    }   
}