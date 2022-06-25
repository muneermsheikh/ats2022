namespace core.Entities
{
    public class Industry: BaseEntity
    {
          public Industry()
          {
          }

          public Industry(string name)
          {
               Name = name;
          }

          public string Name { get; set; }
    }
}