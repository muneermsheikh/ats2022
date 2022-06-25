using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class UserHistoryItemDto
    {
        public int Id { get; set; }
        public int UserContactId { get; set; }
        public string PhoneNo { get; set; }
        public string Subject { get; set; }
        public string CategoryRef { get; set; }
        public DateTime DateOfContact { get; set; }
        public int LoggedInUserId { get; set; }
        public string LoggedInUserName { get; set; }
        public int ContactResult { get; set; }
        public string ContactResultName { get; set; }
        public string GistOfDiscussions { get; set; }
    }
}