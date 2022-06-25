using System;
using System.Collections.Generic;
using core.Entities.HR;

namespace core.Params
{
    public class CandidateHistorySpecParams: ParamPages
    {
        public int? ApplicationNo { get; set; }
        public string AadharNo { get; set; }
        public string PhoneNo { get; set; }
        public int? Id { get; set; }
        public string PartyNameLike { get; set; }
        public DateTime TransactionDateFrom {get; set;}
    }
}