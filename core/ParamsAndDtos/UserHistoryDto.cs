using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core.ParamsAndDtos
{
    public class UserHistoryDto
    {
        public UserHistoryDto()
        {
        }

        public UserHistoryDto(string personName, int personId, string emailId, int applicationNo)
        {
            PersonName = personName;
            PersonId = personId;
            EmailId = emailId;
            ApplicationNo = applicationNo;
        }

        public UserHistoryDto(int id, string partyName, int personId, string emailId, int applicationNo, ICollection<UserHistoryItemDto> historyitems)
        {
            Id = id;
            PersonName = partyName;
            PersonId = personId;
            EmailId = emailId;
            ApplicationNo = applicationNo;
            UserHistoryItems = historyitems;
        }

        public int Id {get; set;}
        public string PersonName {get; set;}
        public string PersonType {get; set;}
        public int PersonId {get; set;}
        public string EmailId {get; set;}
        public string PhoneNo { get; set; }
        public int ApplicationNo {get; set;}
        public ICollection<UserHistoryItemDto> UserHistoryItems { get; set; }
    }
}