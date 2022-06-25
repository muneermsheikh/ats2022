using System;

namespace core.ParamsAndDtos
{
    public class HRExecReportDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int EmployeeId {get; set;}

    }
}