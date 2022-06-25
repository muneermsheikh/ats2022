using System;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.Admin
{
    public class Person
    {
          public Person()
          {
          }

          public Person(string gender, string firstName, string secondName, string familyName, string knownAs, DateTime dOB, 
            string placeOfBirth, string aadharNo, string pPNo, string nationality)
          {
               Gender = gender;
               FirstName = firstName;
               SecondName = secondName;
               FamilyName = familyName;
               KnownAs = knownAs;
               DOB = dOB;
               PlaceOfBirth = placeOfBirth;
               AadharNo = aadharNo;
               PpNo = pPNo;
               Nationality = nationality;
          }

        [Required]
        public string UserType {get; set;}
        [Required]
        public string Gender { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required]         
        public string KnownAs { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AadharNo { get; set; }
        public string PpNo { get; set; } 
        public string Nationality {get; set;}
        [EmailAddress]
        public string Email {get; set;}
        public string UserName {get; set;}
    }
}