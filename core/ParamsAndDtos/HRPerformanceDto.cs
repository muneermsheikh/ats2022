using System;

namespace core.ParamsAndDtos
{
    public class HRPerformanceDto
    {
        public int EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateUpto { get; set; }
        public int PerformanceType { get; set; }
        public string PerformanceParameter { get; set; }
        public string CandidateName { get; set; }
        public int OrderNo { get; set; }
        public string CategoryName { get; set; }
    }
}