using core.Params;

namespace core.ParamsAndDtos
{
    public class SelDecisionSpecParams: ParamPages
    {
        public int? OrderItemId { get; set; }
        public int? CategoryId {get; set;}
        public string CategoryName {get; set;}
        public int? OrderId { get; set; }
        public int? OrderNo { get; set; }
        public int? CandidateId {get; set;}
        public int? ApplicationNo { get; set; }
        public int? CVRefId { get; set; }
        public int[] CVRefIds {get; set;}
        public bool IncludeEmploymentData {get; set;}=false;
        public bool IncludeDeploymentDAta { get; set; }=false;
    }
}