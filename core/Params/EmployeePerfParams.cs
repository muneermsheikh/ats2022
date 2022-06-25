using System;
using System.Collections.Generic;
using core.Entities.Orders;
using core.ParamsAndDtos;

namespace core.Params
{
    public class EmployeePerfParams: ParamPages
    {
        public string Department {get; set;}
        public int? EmployeeId { get; set; }
        public DateTime? DateFrom {get; set;}
        public DateTime? DateUpto {get; set;}
        public string PerformanceType { get; set; }
        public ICollection<EnumTaskType> PerformanceParameters {get; set;}
        public int? OrderId {get; set;}
        public int? OrderItemId { get; set; }
        public int? OrderNo {get; set;}

    }
}