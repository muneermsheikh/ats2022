namespace core.Entities.MasterEntities
{
    public class ReviewItemStatus: BaseEntity
    {
          public ReviewItemStatus()
          {
          }

          public ReviewItemStatus(string itemStatus)
          {
               ItemStatus = itemStatus;
          }

          public string ItemStatus { get; set; }
    }
}