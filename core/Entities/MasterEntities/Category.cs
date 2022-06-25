namespace core.Entities
{
    public class Category: BaseEntity
    {
          public Category()
          {
          }

          public Category(string name)
          {
               Name = name;
          }

          public string Name { get; set; }
    }
}