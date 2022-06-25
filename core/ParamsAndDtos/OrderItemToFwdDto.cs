using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class OrderItemToFwdDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public int ProjectManagerId {get; set;}
        public int CategoryId { get; set; }
        public string CategoryRef { get; set; }
        public string CategoryName {get; set;}
        public int Quantity { get; set; }
        [Required]
        public int MinCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public int Charges { get; set; }
        public string SalaryCurrency { get; set; }
        public int BasicSalary { get; set; }
        public string RemunerationURL {get; set;}
        public string JobDescriptionURL {get; set;}
    }
}