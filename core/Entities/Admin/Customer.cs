using System;
using System.Collections.Generic;
using core.Entities.Admin;

namespace core.Entities
{
    public class Customer: BaseEntity
    {
          public Customer()
          {
          }

          public Customer(string customerType, string customerName, string knownAs, 
            string add, string add2, string city, string pin, string district, 
            string state, string country, string email, string website, 
            string phone, string phone2, ICollection<CustomerIndustry> customerIndustries, 
            ICollection<CustomerOfficial> customerOfficials, ICollection<AgencySpecialty> agencySpecialties)
          {
               CustomerType = customerType;
               CustomerName = customerName;
               KnownAs = knownAs;
               Add = add;
               Add2 = add2;
               City = city;
               Pin = pin;
               District = district;
               State = state;
               Country = country;
               Email = email;
               Website = website;
               Phone = phone;
               Phone2 = phone2;
               CustomerIndustries = customerIndustries;
               CustomerOfficials = customerOfficials;
               AgencySpecialties = agencySpecialties;
          }

        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string KnownAs { get; set; }
        public string Add { get; set; }
        public string Add2 { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email {get; set;}
        public string Website {get; set;}
        public string Phone {get; set;}
        public string Phone2 {get; set;}
        public string LogoUrl {get; set;}
        public DateTime CreatedOn { get; set; }
        public string Introduction { get; set; }
      
        public EnumCustomerStatus CustomerStatus {get; set;}=EnumCustomerStatus.Active;
        public ICollection<CustomerIndustry> CustomerIndustries { get; set; }
        public ICollection<CustomerOfficial> CustomerOfficials { get; set; }
        public virtual ICollection<AgencySpecialty> AgencySpecialties {get; set;}

    }
}