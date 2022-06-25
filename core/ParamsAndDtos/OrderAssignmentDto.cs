using System;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class OrderAssignmentDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CityOfWorking { get; set; }
        public int ProjectManagerId { get; set; }
        public string ProjectManagerPosition { get; set; }
        public int OrderItemId { get; set; }
        public int HrExecId { get; set; }
        public string CategoryRef { get; set; }
        public int CategoryId {get; set;}
        public string CategoryName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public DateTime CompleteBy { get; set; }
        public int PostTaskAction {get; set;}= (int) EnumPostTaskAction.DoNotComposeAnyMessage;
    }
}