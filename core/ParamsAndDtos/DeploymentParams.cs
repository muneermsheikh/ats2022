using System;
using core.Params;

namespace core.ParamsAndDtos
{
    public class DeploymentParams: ParamPages
    {
        public int OrderDetailId { get; set; }
        public int OrderId {get; set;}
    }
}