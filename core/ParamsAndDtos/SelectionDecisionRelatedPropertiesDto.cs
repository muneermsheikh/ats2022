namespace core.ParamsAndDtos
{
    public class SelectionDecisionRelatedPropertiesDto
    {
        public int CVRefId { get; set; }
        public int OrderItemId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName {get; set;}
        public int OrderId { get; set; }
        public int OrderNo {get; set;}
        public int ApplicationNo {get; set;}
        public string CandidateName {get; set;}
    }
}