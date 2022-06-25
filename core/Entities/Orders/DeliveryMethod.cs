namespace core.Entities.Orders
{
    public class DeliveryMethod: BaseEntity
    {
        public string ShortName { get; set; }
        public string DeliveryTime { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        
    }
}