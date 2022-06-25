namespace core.Entities.MasterEntities
{
    public class Qualification: BaseEntity
    {
          public Qualification()
          {
          }

          public Qualification(string name)
          {
               Name = name;
          }

          public string Name { get; set; }
    }
}