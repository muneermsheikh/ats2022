namespace core.Entities.Admin
{
    public class EmployeeHRSkill: BaseEntity
    {
        public EmployeeHRSkill()
        {
        }

        public EmployeeHRSkill(int categoryId, int industryId, int skillLevel)
        {
            CategoryId = categoryId;
            IndustryId = industryId;
            SkillLevel = skillLevel;
        }

        public EmployeeHRSkill(int employeeId, int categoryId, int industryId, int skillLevel)
        {
            EmployeeId = employeeId;
            CategoryId = categoryId;
            IndustryId = industryId;
            SkillLevel = skillLevel;
        }

        public int EmployeeId { get; set; }
        public int CategoryId { get; set; }        
        public int IndustryId {get; set;}
        public int SkillLevel {get; set;}
        //public Employee Employee {get; set;}
    }
}