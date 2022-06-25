using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class CandidateAssessmentAndChecklistDto
    {
        public CandidateAssessmentAndChecklistDto(CandidateAssessment assessed, ChecklistHRDto checklistHRDto)
        {
            //AssessedDto = assessedDto;
            Assessed = assessed;
            ChecklistHRDto = checklistHRDto;
        }

        //public CandidateAssessedDto AssessedDto {get; set;}
        public CandidateAssessment Assessed {get; set;}
        public ChecklistHRDto ChecklistHRDto {get; set;}
        public string ErrorString {get; set;}
    }
}