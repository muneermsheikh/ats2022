    using System.Runtime.Serialization;

namespace core.Entities.Process
{
    public enum EnumDeployStatus
    {
        [EnumMember(Value="None")] None = 0,
        [EnumMember(Value="Selected")] Selected = 200,
        [EnumMember(Value="Offer Letter Accepted")] OfferLetterAccepted = 300,
        [EnumMember(Value="Referred for Medical Test")] ReferredForMedical=400,
        [EnumMember(Value="Medically Declared Fit")] MedicallyFit=500,
        [EnumMember(Value="Medically Unfit")] MedicallyUnfit=600,
        [EnumMember(Value="Visa Documents Prepared")] VisaDocsPrepared=1000,
        [EnumMember(Value ="Visa Documents Submitted")] VisaDocsSubmitted=1100,
        [EnumMember(Value="Visa Query Raised")] VisaQueryRaised=1150,
        [EnumMember(Value="Visa Denied")] VisaDenied=1250,
        [EnumMember(Value="Visa Received")] VisaReceived=1300,
        [EnumMember(Value="Emigration Docs Lodged Online")] EmigDocsLodgedOnLine=2000,
        [EnumMember(Value="Emigration - Query Received")] EmigDocsQueryRecd=2050,
        [EnumMember(Value="PP Submitted to Emigration office")] EmigDocsPPSubmitted=2150,
        [EnumMember(Value="Emigration Denied")] EmigrationDenied=2250,
        [EnumMember(Value="Emigration Granted")] EmigrationGranted=2300,
        [EnumMember(Value="Travel Ticket booked")]TravelTicketBooked=3000,
        [EnumMember(Value = "Travel Canceled")] TravelCanceled=3050,
        [EnumMember(Value="Traveled")]Traveled=3200,
        [EnumMember(Value="Arrival acknowledged by client")] ArrivalAcknowledgedByClient=3300,
        [EnumMember(Value="Concluded")] Concluded=10000
    }
}