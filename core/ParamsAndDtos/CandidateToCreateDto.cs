using System;
using System.Collections.Generic;
using core.Entities.Admin;
using core.Entities.Users;

namespace core.ParamsAndDtos
{
     public class CandidateToCreateDto
    {
        public CandidateToCreateDto()
        {
        }

        public string AppUserId {get; set;}
        public int ApplicationNo { get; set; }
        public Person Person {get; set;}
        public string AadharNo { get; set; }
        public string City { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
        //public ICollection<Address> Addresses { get; set; }
        public ICollection<UserQualification> UserQualifications { get; set; }
        public ICollection<UserProfession> UserProfessions {get; set;}
        public ICollection<UserPassport> UserPassports {get; set;}
        public ICollection<UserAttachment> UserAttachments { get; set; }

    }
}