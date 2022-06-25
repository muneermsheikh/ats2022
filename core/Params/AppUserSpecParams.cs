namespace core.Params
{
    public class AppUserSpecParams: ParamPages
    {
        public int? ProfessionId { get; set; }
        public string City {get; set;}
        public string District {get; set;}
        public string State {get; set;}
        public string Email {get; set;}
        public string Mobile {get; set;}

        public bool IncludeAttachments {get; set;}
        public bool IncludeUserPhones {get; set;}
        public bool IncludeUserQualifications {get; set;}
        public bool IncludeUserProfessions { get; set; }
        public bool IncludeUserPassorts { get; set; }
        public bool IncludeEntityAddresses { get; set; }
        public bool IncludeUserExperiences { get; set; }
    }
}