using System;
using core.Params;

namespace core.Params
{
    public class InterviewSpecParams: ParamPages
    {
        public int? InterviewId { get; set; }
        public int? InterviewItemId {get; set;}
        public int? OrderId {get; set;}
        public int? OrderItemId {get; set;}
        public int? CategoryId {get; set;}
        public string CategoryName {get; set;}
        public int? CustomerId {get; set;}
        public string CustomerName {get; set;}
        public int? ApplicationNo {get; set;}
        public string InterviewStatus {get; set;}
        public bool IncludeItems {get; set;}

    }
}