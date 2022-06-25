using System;
using core.Entities.Process;

namespace core.ParamsAndDtos
{
    public class DeployAddedDto
    {
        public int Id { get; set; }
        public int CVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public EnumDeployStatus StageId { get; set; }
        public EnumDeployStatus NextStageId { get; set; }
        public DateTime NextStageDate { get; set; }
    }
}