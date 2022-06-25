using System;
using core.Entities.Admin;

namespace core.Params
{
    public class CustomerReviewSpecParams: ParamPages
    {
          public CustomerReviewSpecParams()
          {
          }

        public int? CustomerId { get; set; }
        public string CurrentStatus {get; set;}
        public string CustomerNameLike {get; set;}
        public int? CustomerReviewItemDataId {get; set;}
        public DateTime? ReviewTransactionDate {get; set;}
    }
}