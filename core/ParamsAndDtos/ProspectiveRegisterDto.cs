using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Users;

namespace core.ParamsAndDtos
{
    public class ProspectiveRegisterDto
    {
        public int Id { get; set; }
        public int AppUserId {get; set;}
        public string UserType { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string KnownAs { get; set; }
        public int ReferredBy { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public ICollection<EntityAddress> EntityAddresses { get; set; }
        public ICollection<UserPhone> UserPhones {get; set;}
        public ICollection<UserProfession> UserProfessions {get; set;}
    }
}