using core.Params;

namespace core.ParamsAndDtos
{
    public class ContractReviewParams: ParamPages
    {
        public string CurrentUsername { get; set; }
        public string CustomerNameLike { get; set; }
        public int OrderId {get; set;}
        public int OrderNo { get; set; }
        public bool CurrentMonth { get; set; }
        public string DateBegin { get; set; }
        public string DateEnd { get; set; }
        public string OrderBy { get; set; }
        public int ReviewStatus { get; set; }
        /*
        professionIds: number[]=[];
        industryIds: number[]=[];
        */
    }
}