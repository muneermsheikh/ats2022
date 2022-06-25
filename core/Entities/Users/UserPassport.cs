using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Users
{
    public class UserPassport: BaseEntity
    {
        public UserPassport()
        {
        }

        public UserPassport(string passportNo, string nationality, DateTime validity)
        {
            PassportNo = passportNo;
            Nationality = nationality;
            Validity = validity;
        }
        public UserPassport(int candidateId, string passportNo, string nationality, DateTime validity)
        {
            CandidateId = candidateId;
            PassportNo = passportNo;
            Nationality = nationality;
            Validity = validity;
        }

        public int CandidateId { get; set; }
        public string PassportNo { get; set; }
        public string Nationality {get; set;}
        public DateTime? IssuedOn { get; set; }
        public DateTime Validity { get; set; }
        public bool IsValid { get; set; }=true;
        public bool Ecnr {get; set;}=false;
        //public Candidate Candidate {get; set;}
    }
}