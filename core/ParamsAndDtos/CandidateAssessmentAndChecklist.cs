using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class CandidateAssessmentAndChecklist
    {
        public CandidateAssessment CandidateAssessment {get; set;}
        public ChecklistHR ChecklistHR {get; set;}
    }
}