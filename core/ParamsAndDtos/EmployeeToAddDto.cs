using System;
using core.Entities.Identity;

namespace core.ParamsAndDtos
{
    public class EmployeeToAddDto
    {
        public int Id {get; set;}
        public int AppUserId { get; set; }
        public string Gender { get; set; }
        //public string EmployeeName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public string KnownAs { get; set; }
        public string Position { get; set; }
        public DateTime DOB {get; set;}
        public string PlaceOfBirth { get; set; }
        public string AadharNo { get; set; }
        public string Nationality {get; set;}
        public string Email {get; set;}
        public DateTime DOJ {get; set;}
        public string Department { get; set; }
        public string OfficialPhoneNo { get; set; }
        public string OfficialMobileNo { get; set; }
        public string OfficialEmailAddress {get; set;}
        public string Add {get; set;}
        public string Address {get; set;}
        public string City {get; set;}
        public string Pin {get; set;}
        public string Country {get; set;}
        public string Password {get; set;}
        public AppUser appUser {get; set;}
        
        public string UserName { get; set; }
        public string Remarks {get; set;}
    }
}