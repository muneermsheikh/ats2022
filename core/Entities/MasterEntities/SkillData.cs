namespace core.Entities.MasterEntities
{
    public class SkillData: BaseEntity
    {
          public SkillData()
          {
          }

          public SkillData(string skillName)
          {
               SkillName = skillName;
          }

          public string SkillName { get; set; }
    }
}