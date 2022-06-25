using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class SelectionDecisionMessageParamDto
    {
        public SelectionDecision SelectionDecision { get; set; }
        public bool DirectlySendMessage { get; set; }
    }
}