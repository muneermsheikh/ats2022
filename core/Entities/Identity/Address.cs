using System;
using System.ComponentModel.DataAnnotations;
using core.Entities.Users;

namespace core.Entities.Identity
{
    public class Address: BaseEntity
    {
          public Address()
          {
          }

          public Address(string add, string streetAdd, string location, string city, string pin, string state, string country)
          {
               Add = add;
               StreetAdd = streetAdd;
               Location = location;
               City = city;
               Pin = pin;
               State = state;
               Country = country;
          }

        public string AddressType { get; set; }="R";
        [Required, MaxLength(1)]
        public string Gender { get; set; }="M";
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public DateTime DOB { get; set; }
        public string Add { get; set; }
        public string StreetAdd { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string Country { get; set; }="India";
        //[Required]
        //public string AppUserId { get; set; }
        //public AppUser AppUser { get; set; }
          
    }
}