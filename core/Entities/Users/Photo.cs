namespace core.Entities.Users
{
    public class Photo: BaseEntity
    {
        public int AppUserId { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}