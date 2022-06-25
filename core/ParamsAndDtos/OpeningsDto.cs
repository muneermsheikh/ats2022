using System;

namespace core.ParamsAndDtos
{
    public class OpeningsDto
    {
          public OpeningsDto()
          {
          }

          public OpeningsDto(int orderItemId, int selected, int rejected, int canceledAfterSelection, 
            int clBalance, string customerName, int orderNo, DateTime orderDate, DateTime targettedDate, 
            int professionId, string professionName, int quantity, bool requireAssess, DateTime completeBefore)
          {
               OrderItemId = orderItemId;
               Selected = selected;
               Rejected = rejected;
               CanceledAfterSelection = canceledAfterSelection;
               ClBalance = clBalance;
               CustomerName = customerName;
               OrderNo = orderNo;
               OrderDate = orderDate;
               TargettedDate = targettedDate;
               ProfessionId = professionId;
               ProfessionName = professionName;
               Quantity = quantity;
               RequireAssess = requireAssess;
               CompleteBefore = completeBefore;
          }

        public int OrderItemId { get; set; }
        public int Selected { get; set; }
        public int Rejected { get; set; }
        public int CanceledAfterSelection { get; set; }
        public int ClBalance { get; set; }

        public string CustomerName { get; set; }
        public int OrderNo {get; set;}
        public DateTime OrderDate { get; set; }
        public DateTime TargettedDate { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int Quantity { get; set; }
        public bool RequireAssess {get; set;}
        
        public DateTime CompleteBefore { get; set; }
    }
}