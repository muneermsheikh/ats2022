using System;
using System.Linq;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class EmployeeSpecs: BaseSpecification<Employee>
    {
        public EmployeeSpecs(EmployeeSpecParams empParams)
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
            if(empParams.IncludeHRSkills) AddInclude(x => x.HrSkills);
            if(empParams.IncludeOtherSkills) AddInclude(x => x.OtherSkills);
            if(empParams.IncludePhones) AddInclude(x => x.EmployeePhones);
            if (empParams.IncludeQualifications) AddInclude(x => x.EmployeeQualifications);

            ApplyPaging(empParams.PageSize * (empParams.PageIndex - 1), empParams.PageSize);

            if (!string.IsNullOrEmpty(empParams.Sort))
            {
                switch (empParams.Sort)
                {
                    case "PositionAsc": AddOrderBy(x => x.Position); break;
                    case "PositionDesc": AddOrderByDescending(x => x.Position); break;
                    case "DepartmentAsc": AddOrderBy(x => x.Department); break;
                    case "DepartmentDesc": AddOrderByDescending(x => x.Department); break;
                    default: 
                        AddOrderBy(x => x.Id);
                    break;
                }   
            }
        }

        public EmployeeSpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}