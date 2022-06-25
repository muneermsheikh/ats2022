using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Users;

namespace core.Entities.HR
{
    public class CandidateBriefDto
    {
          public CandidateBriefDto()
          {
          }

          public CandidateBriefDto(int id, string gender, int applicationNo, string aadharNo, string fullName, string city, int referredById, string candidateStatusName)
        {
            Id = id;
            Gender = gender;
            ApplicationNo = applicationNo;
            AadharNo = aadharNo;
            FullName = fullName;
            City = city;
            ReferredById = referredById;
            CandidateStatusName = candidateStatusName;
        }

        public bool Checked {get; set;}
        public int Id { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string MobileNo {get; set;}
        public string PassportNo { get; set;}
        public int ApplicationNo { get; set; }
        public string AadharNo {get; set;}

        public string City { get; set; }
        public int ReferredById { get; set; }
        public string ReferredByName { get; set; }
        public string CandidateStatusName { get; set; }
        public ICollection<UserProfession> UserProfessions {get; set;}
    }
}