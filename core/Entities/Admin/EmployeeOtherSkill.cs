namespace core.Entities.Admin
{
    public class EmployeeOtherSkill: BaseEntity
    {
        public EmployeeOtherSkill()
        {
        }

        public EmployeeOtherSkill(int skillDataId, int skillLevel, bool isMain)
        {
            SkillDataId = skillDataId;
            SkillLevel = skillLevel;
            IsMain = isMain;
        }

        public EmployeeOtherSkill(int employeeId, int skillDataId, int skillLevel, bool isMain)
        {
            EmployeeId = employeeId;
            SkillDataId = skillDataId;
            SkillLevel = skillLevel;
            IsMain = isMain;
        }

        public int EmployeeId { get; set; }
        public int SkillDataId { get; set; }
        public int SkillLevel { get; set; }
        public bool IsMain { get; set; }
        //public Employee Employee {get; set;}
    }
}