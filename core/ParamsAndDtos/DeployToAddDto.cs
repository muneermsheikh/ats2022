using System;
using System.ComponentModel.DataAnnotations;

namespace core.ParamsAndDtos
{
    public class DeployToAddDto
    {
        [Required]
        public int CVRefId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public int DeployStatusId { get; set; }
        public int DeployStageId { get; set; }
        public int NextDeployStageId { get; set; }
        public DateTime NextDeployStageEstimatedDate { get; set; }
    }
}