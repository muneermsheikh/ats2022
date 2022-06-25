namespace core.Entities.MasterEntities
{
    public class DeployStage: BaseEntity
    {
        public string Status { get; set; }
        public int Sequence { get; set; }
        public int NextDeployStageSequence { get; set; }
    }
}