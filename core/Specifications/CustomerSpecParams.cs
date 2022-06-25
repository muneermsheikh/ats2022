namespace core.Specifications
{
    public class CustomerSpecParams: CommonSpecParams
    {
        public string CustomerType {get; set;}
        public string CustomerCityName { get; set; }
        public int? CustomerIndustryId {get; set;}
        public bool IncludeOfficials {get; set;}=false;
        public bool IncludeIndustries {get; set;}=true;
        
        public int? CustomerOfficialId { get; set; }
        public int? IndustryId { get; set; }
        private string _search;
    }
}