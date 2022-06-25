using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace core.Entities.Identity
{
     public class AppUser: IdentityUser<int>
    {
        public string UserType {get; set;}
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string DisplayName { get; set; }
        public int loggedInEmployeeId {get; set;}
        //public Address Address { get; set; }
        
        //public int UserPassportId { get; set; }
        //public UserPassport UserPassport {get; set;}
        public ICollection<AppUserRole> UserRoles { get; set; }
        
        //public ICollection<UserLike> LikedByUsers { get; set; }
        //public ICollection<UserLike> LikedUsers { get; set; }
        //public ICollection<Message> MessagesSent { get; set; }
        //public ICollection<Message> MessagesReceived { get; set; }
    }
}