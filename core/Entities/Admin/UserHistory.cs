using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.Admin
{
    public class UserHistory: BaseEntity
    {
        public UserHistory()
        {
        }

        public UserHistory(int personid, string personname, ICollection<UserHistoryItem> items)
        {
            PersonId = personid;
            PersonName = personname;
            UserHistoryItems = items;
        }
        public UserHistory(int personid, string personname)
        {
            PersonId = personid;
            PersonName = personname;
        }

        public string PersonType {get; set;}
        public int PersonId {get; set;}
        public int? ApplicationNo {get; set;}
        public string PersonName {get; set;}
        public string PhoneNo {get; set;}
        public string EmailId {get; set;}
        public DateTime CreatedOn {get; set;}
        public ICollection<UserHistoryItem> UserHistoryItems {get; set;}
    }
}