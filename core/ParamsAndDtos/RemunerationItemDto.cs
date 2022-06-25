namespace core.ParamsAndDtos
{
    public class RemunerationItemDto
    {
        public int Id { get; set; }
        public string CategoryRefAndName { get; set; }
        public int Quantity { get; set; }
        public string Salary { get; set; }
        public int? ContractPeriod { get; set; }
        public string Food { get; set; }
        public string Housing { get; set; }
        public string Transport { get; set; }
        public string Other { get; set; }
        public int? AnnualLeave { get; set; }
        public int? LeaveAfter { get; set; }
    }
}