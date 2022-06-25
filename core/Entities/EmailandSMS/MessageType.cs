namespace core.Entities.EmailandSMS
{
    public class MessageType: BaseEntity
    {
        public string Name { get; set; }
    }


    public enum EnumMessageType
    {
        CVAcknowledgementByEMail,
        CVAcknowledgementbySMS, 
        OrderAcknowledgement,
        OrderAssessmentQDesigning,
        TaskAssignmentToHRExecToShortlistCV,
        TaskAssignmentToHRSupToReviewCV,
        TaskAssignmentToHRMToReviewCV,
        CVForwardingToDocControllerToFwdCVToClient,
        Publish_CVFwdToClient,
        Publish_CVReviewedByHRSup,
        Publish_CVReviewedByHRManager,
        AdviseTOHRSup_CVFwdToHRM,
        CVForwardingToClient, 
        SelectionAdvisebyemail, 
        SelectionAdvisebySMS,
        RejectionAdviseByMail, 
        RejectionAdvisebySMS, 
        MedicalExaminationAdvisebyEmail,
        MedicalExaminationAdvisebySMS, 
        VisaDocumentationAdvisebyEmail, 
        VisaDocumentationAdvisebySMS, 
        TravelBookingAdviseByEmail,
        TravelBookingAdviseBySMS,
        RequirementForwardToHRDept,
        DLForwardToAgents,
        CandidateFollowup
    }
}