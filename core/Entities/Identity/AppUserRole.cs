using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace core.Entities.Identity
{
    
    //[NotMapped]
    public class AppUserRole : IdentityUserRole<int>
    {
        public int Id {get; set;}
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }

}