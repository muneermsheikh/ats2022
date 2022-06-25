using System;
using core.Entities.Process;

namespace core.ParamsAndDtos
{
    public class DeployPostDto
    {
        public int CVRefId { get; set; }
        public EnumDeployStatus StageId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}