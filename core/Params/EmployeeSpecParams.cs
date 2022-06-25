using System;
using core.Entities.Admin;

namespace core.Params
{
    public class EmployeeSpecParams: ParamPages
    {
          public EmployeeSpecParams()
          {
          }

        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string FamilyName {get; set;}
        public string Department {get; set;}
        public DateTime? DOJ {get; set;}
        public int? SkillLevel {get; set;}
        public int? SkillDataId {get; set;}
        public int? OtherSkillLevel {get; set;}
        public string Position {get; set;}
        public bool IncludeHRSkills {get; set;}=false;
        public bool IncludeOtherSkills {get; set;}=false;
        public bool IncludeQualifications {get; set;}=false;
        public bool IncludePhones {get; set;}=false;
        
    }
}