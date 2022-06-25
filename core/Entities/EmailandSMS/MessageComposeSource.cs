namespace core.Entities.EmailandSMS
{
    public class MessageComposeSource: BaseEntity
    {
        public string MessageType { get; set; }
        public string Mode {get; set;}
        public int SrNo { get; set; }
        public string LineText { get; set; }
    }
}