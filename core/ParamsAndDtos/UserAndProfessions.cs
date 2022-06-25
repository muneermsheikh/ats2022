using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class UserAndProfessions
    {
        public int CandidateId { get; set; }
        public ICollection<Prof> CandidateProfessions {get; set;}
    }

    public class Prof
    {
        public int CategoryId { get; set; }
        public int IndustryId { get; set; }
        public bool IsMain { get; set; }
    }
}