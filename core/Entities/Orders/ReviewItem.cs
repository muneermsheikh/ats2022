namespace core.Entities.Orders
{
    public class ReviewItem: BaseEntity
    {
        public ReviewItem()
        {
        }

          public ReviewItem(bool assessed, int orderItemId, int srNo, string reviewParameter, bool response, bool isResponseBoolean, bool isMandatoryTrue)
          {
               Assessed = assessed;
               OrderItemId = orderItemId;
               SrNo = srNo;
               ReviewParameter = reviewParameter;
               Response = response;
               IsResponseBoolean = isResponseBoolean;
               IsMandatoryTrue = isMandatoryTrue;
          }

        public int OrderItemId { get; set; }
        public bool? Assessed {get; set;}=null;
        public int ContractReviewItemId { get; set; }
        public int SrNo { get; set; }
        public string ReviewParameter { get; set; }
        public bool? Response { get; set; }=null;
        public string ResponseText {get; set;}
        public bool IsResponseBoolean {get; set;}
        public bool IsMandatoryTrue { get; set; }=false;
        public string Remarks { get; set; }

    }
}