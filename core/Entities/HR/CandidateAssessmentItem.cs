namespace core.Entities.HR
{
    public class CandidateAssessmentItem: BaseEntity
    {
        public CandidateAssessmentItem()
          {
          }

          public CandidateAssessmentItem(int questionNo, string assessmentparameter, string question, bool isMandatory, int maxPoints)
          {
               QuestionNo = questionNo;
               AssessmentParameter = assessmentparameter;
               Question = question;
               IsMandatory = isMandatory;
               MaxPoints = maxPoints;
          }

          public CandidateAssessmentItem(int candidateAssessmentId, int questionNo, string assessmentparameter, 
            string question, bool isMandatory, int maxPoints, int points, string remarks)
          {
               CandidateAssessmentId = candidateAssessmentId;
               QuestionNo = questionNo;
               AssessmentParameter = assessmentparameter;
               Question = question;
               IsMandatory = isMandatory;
               MaxPoints = maxPoints;
               Points = points;
               Remarks = remarks;
          }

        public int CandidateAssessmentId { get; set; }
        public int QuestionNo { get; set; }
        public string AssessmentParameter { get; set; }
        public string Question { get; set; }
        public bool IsMandatory { get; set; }
        public bool Assessed {get; set;}
        public int MaxPoints { get; set; }
        public int Points { get; set; }   
        
        public string Remarks { get; set; }
    }
}