using System;
using core.Entities.Admin;
using core.Params;

namespace core.Params
{
    public class UserHistoryParams
    {
          public UserHistoryParams()
          {
          }

        public UserHistoryParams(int? id, string personType, string personName, int? personId, int? applicationNo, string emailId, string mobileNo, bool createNewIfNull)
        {
            Id = id;
            PersonType = personType;
            PersonName = personName;
            PersonId = personId;
            ApplicationNo = applicationNo;
            EmailId = emailId;
            MobileNo = mobileNo;
            CreateNewIfNull = createNewIfNull;
        }

        public int? Id {get; set;}
        public string PersonType {get; set;}
        public string PersonName { get; set; }
        public int? PersonId {get; set;}
        public int? ApplicationNo {get; set;}
        public string EmailId {get; set;}
        public string MobileNo {get; set;}
        public DateTime DateAdded { get; set; }
        public string CategoryRef { get; set; }
        public bool CreateNewIfNull {get; set;}=false;
    }
}