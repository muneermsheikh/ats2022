using System.Runtime.Serialization;

namespace core.Entities.Orders
{
    public enum EnumTaskType
    {
        [EnumMember(Value="Order Registration - by Doc Controller Admin")]  OrderRegistration,  //0
        [EnumMember(Value="Contract Review - by Admin")]  ContractReview,     //1
        [EnumMember(Value="Order Assignment to Project Manager")] OrderAssignmentToProjectManager,    //2
        [EnumMember(Value="Design Order Assessment Questions")] DesignOrderAssessmentQ,     //3
        [EnumMember(Value="Assign Requirements to HR Exec - by HR Sup")] AssignTaskToHRExec,     //4
        [EnumMember(Value="Submit CV to HR Supervisor for review")] SubmitCVToHRSupForReview,   //5
        [EnumMember(Value="Submit CV to HR Mgr for Review")] SubmitCVToHRMMgrForReview,  //6
        [EnumMember(Value ="HRM Reviewed the CV")] HRMReviewedCV,      //7
        [EnumMember(Value="Submit CV to Doc Controller Admin to forward CV to Clients")]  SubmitCVToDocControllerAdmin,  //8
        [EnumMember(Value="task to Doc Controller (admin) - Selection followup")] SelectionFollowupByDocControllerAdmin,      //9
        [EnumMember(Value="Selection Rejection Registration by Doc Controller Admin")] SelctionRegistrationByDocControllerAdmin,   //10
        [EnumMember(Value="Medical Test mobilization")] MedicalTestsMobilization,           //11
        [EnumMember(Value="Visa Documents Compilation for KSA, by Visa Executive")]  VisaDocsKSACompilation,         //12
        [EnumMember(Value="Visa Documents Compilation for Non KSA, by Visa Excutive")] VisaDocsNonKSACompilation,          //13
        [EnumMember(Value="Emigration Application Lodging, by Emig Executive")] EmigrationAppLodging,           //14
        [EnumMember(Value="Travel tickets booking, by Ticketing Exec")] TravelTicketBooking,        //15
        [EnumMember(Value ="Order edited advise")]  OrderEditedAdvise,           //16
        [EnumMember(Value ="Forward CV to Customer")] CVForwardToCustomers, //17
        [EnumMember(Value ="Offer Letter Acceptance by candidate")] OfferLetterAcceptance,      //18
        [EnumMember(Value ="Medically Fit")] MedicallyFit,      //19
        [EnumMember(Value = "None")] None,      //20
        [EnumMember(Value = "Visa Docs submission")] VisaDocSubmission,     //21
        [EnumMember(Value = "Visa Received")] VisaReceived,         //22
        [EnumMember(Value = "Emigration Granted")] EmigrationGranted,       //23
        [EnumMember(Value = "Emigration Docs Submitted")] EmigrationDocsSubmitted,      //24
        [EnumMember(Value = "Traveled")] Traveled,      //25
        [EnumMember(Value = "Arrival acknowledged by Client")] ArrivalAcknowledgedByClient,      //26
        [EnumMember(Value = "Referred for Medical tests")] ReferredForMedicalTest      //27

    }
}
