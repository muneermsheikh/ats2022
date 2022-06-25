namespace core.Entities
{
    public class CustomerIndustry: BaseEntity
    {
        public int CustomerId { get; set; }
        public int IndustryId { get; set; }
        public string Name {get; set;}
        //public Customer Customer {get; set;}
    }
}