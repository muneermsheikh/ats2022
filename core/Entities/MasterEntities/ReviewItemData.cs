namespace core.Entities.MasterEntities
{
    public class ReviewItemData: BaseEntity
    {
        public ReviewItemData()
        {
        }

        public ReviewItemData(int srNo, string reviewParameter, bool isResponseBoolean)
        {
            SrNo = srNo;
            ReviewParameter = reviewParameter;
            IsResponseBoolean = isResponseBoolean;
        }

        public int SrNo { get; set; }
        public string ReviewParameter { get; set; }
        public bool IsResponseBoolean {get; set;}
        public bool Response {get; set;}
        public bool IsMandatoryTrue {get; set;}
    }
}