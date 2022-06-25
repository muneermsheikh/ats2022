using System.ComponentModel.DataAnnotations;
using core.Entities.Users;

namespace core.Entities
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string FirstName {get; set;}
        public string LastName { get; set; }
        public string Add { get; set; }
        public string StreetAdd { get; set; }
        public string Location { get; set; }
        [Required]
        public string City { get; set; }
        public string Pin { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }="India";
        //[Required]
        public string AppUserId { get; set; }
        [Required ]
        public bool IsMain { get; set;} 
    }
}