using System;
using core.Entities.Admin;

namespace core.ParamsAndDtos
{
    public class UserContactDto
    {
          public UserContactDto()
          {
          }

          public UserContactDto(int id, string candidateName, int applicationNo, string passportno, string aadharno, string phoneno)
          {
               Id = id;
               CandidateName = candidateName;
               ApplicationNo = applicationNo;
               PassportNo = passportno;
               AadharNo = aadharno;
               PhoneNo = phoneno;
          }

        public int Id {get; set;}
        public string CandidateName { get; set; }
        public int ApplicationNo {get; set;}
        public string PassportNo {get; set;}
        public string AadharNo {get; set;}
        public string PhoneNo {get; set;}
        
    }
}