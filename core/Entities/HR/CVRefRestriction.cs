using System;

namespace core.Entities.HR
{
    public class CVRefRestriction: BaseEntity
    {
        public int OrderItemId { get; set; }
        public EnumOrderItemRefRestriction restrictionReason { get; set; }
        public int RestrictedById {get; set;}
        public DateTime RestrictedOn {get; set;}
        public bool RestrictionLifted {get; set;}
        public int RestrictionLiftedById {get; set;}
        public DateTime RestrictionLiftedOn {get; set;}
        public string Remarks {get; set;}
    }
}