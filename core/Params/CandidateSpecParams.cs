using System;
using System.Collections.Generic;
using core.Entities.HR;

namespace core.Params
{
    public class CandidateSpecParams: ParamPages
    {
        public int? CandidateId {get; set;}
        public int? AppUserId {get; set;}
        public string Email {get; set;}
        public string Mobile {get; set;}
        public string PassportNo {get; set;}
        public int? ApplicationNoFrom {get; set;}
        public int? ApplicationNoUpto {get; set;}
        public DateTime? RegisteredFrom {get; set;}
        public DateTime? RegisteredUpto {get; set;}
        public int? ProfessionId { get; set; }
        public int? IndustryId { get; set; }
        public int? AssociateId {get; set;}   //assocaite id
        public string CandidateStatus {get; set;}
        public string City {get; set;}
        public string District {get; set;}
        public string State {get; set;}
        public int? AgentId {get; set;}
        public bool IncludeAttachments {get; set;}
        public bool IncludeUserPhones {get; set;}
        public bool IncludeUserQualifications {get; set;}
        public bool IncludeUserProfessions { get; set; }
        public bool IncludeUserPassorts { get; set; }
        public bool IncludeEntityAddresses { get; set; }
        public bool IncludeUserExperiences { get; set; }
    }
}