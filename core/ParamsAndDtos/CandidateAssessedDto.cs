using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class CandidateAssessedDto
    {
        public int Id {get; set;}
        public int OrderItemId { get; set; }
        public string OrderItemRef {get; set;}
        public bool requireInternalReview {get; set;}
        public string CustomerName {get; set;}
        public int CandidateId {get; set;}
        public string CandidateName {get; set;}
        public int ApplicationNo {get; set;}
        
        public int AssessedById { get; set; }
        public string AssessedByName {get; set;}
        public string AssessedResult {get; set;}
        public string checklistedByName {get; set;}
        public string Charges {get; set;}

        public DateTime AssessedOn { get; set; }
        public string Remarks { get; set; }

    }
}