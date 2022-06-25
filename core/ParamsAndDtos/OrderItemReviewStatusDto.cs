namespace core.ParamsAndDtos
{
    public class OrderItemReviewStatusDto
    {
        public int OrderItemId { get; set; }
        public int CtOfSelected { get; set; }
        public int CtOfRejected { get; set; }
        public int CtOfNotReviewed { get; set; }
    }
}