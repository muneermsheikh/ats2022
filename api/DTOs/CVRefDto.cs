using System;

namespace api.DTOs
{
    public class CVRefDto
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public DateTime ReferredOn { get; set; }
        public string RefStatus { get; set; }
        public int Charges {get; set;}

    }
}