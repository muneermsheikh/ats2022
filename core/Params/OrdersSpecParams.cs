using System;

namespace core.Params
{
    public class OrdersSpecParams: ParamPages
    {
        public int? Id { get; set; }
        public int? CustomerId { get; set; }
        public int? OrderId {get; set;}
        //public int? OrderNo {get; set;}
        public int? OrderNoFrom {get; set;}
        public int? OrderNoUpto {get; set;}
        public DateTime? OrderDateFrom {get; set;}
        public DateTime? OrderDateUpto { get; set; }
        public string CityOfWorking {get; set;}
        public int? CategoryId { get; set; }
        public int? Status { get; set; }
    }
}