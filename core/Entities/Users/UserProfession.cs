using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Users
{
    public class UserProfession:  BaseEntity
    {
        public UserProfession()
        {
        }

        public UserProfession(int candidateId, int categoryId, int industryId, bool isMain)
        {
            CandidateId = candidateId;
            CategoryId = categoryId;
            IndustryId = industryId;
            IsMain = isMain;
        }
        public UserProfession(int categoryId, string profession, int industryId, bool isMain)
        {
            CategoryId = categoryId;
            IndustryId = industryId;
            Profession = profession;
            IsMain = isMain;
        }


        public int CandidateId { get; set; }
        public int CategoryId { get; set; }
        public string Profession   { get; set; }
        public int IndustryId { get; set; }
        public bool IsMain { get; set; }
        //public Candidate Candidate {get; set;}
    }
}