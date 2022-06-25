using core.Entities.Users;

namespace core.ParamsAndDtos
{
    public class CandidateMessageParamDto
    {
        public Candidate Candidate { get; set; }
        public bool DirectlySendMessage { get; set; }
    }
}