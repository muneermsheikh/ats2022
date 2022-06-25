using System;
using System.Linq;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class EmployeeForCountSpecs: BaseSpecification<Employee>
    {
        public EmployeeForCountSpecs(EmployeeSpecParams empParams)
            : base(x => 
               (string.IsNullOrEmpty(empParams.Search) || 
                  (x.FirstName.ToLower().ToLower()
                  + x.FamilyName.ToLower()).Contains(empParams.Search.ToLower())) &&
                (string.IsNullOrEmpty(empParams.Position) ||
                    x.Position.ToLower() == empParams.Position) &&
                (!empParams.Id.HasValue ||  x.Id == empParams.Id) &&
                (string.IsNullOrEmpty(empParams.Department) ||
                    x.Department.ToLower() == empParams.Department.ToLower()) &&
                (!empParams.SkillDataId.HasValue || 
                    x.OtherSkills.Select(s => s.SkillDataId).ToList().Contains((int)empParams.SkillDataId)) &&
                (!empParams.OtherSkillLevel.HasValue || 
                    x.OtherSkills.Select(s => s.SkillLevel).ToList().Contains((int)empParams.OtherSkillLevel))
            )
        {
        }

        public EmployeeForCountSpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}