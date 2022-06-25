using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities;
using core.Entities.Admin;

namespace core.ParamsAndDtos
{
    public class RegisterCustomerDto
    {
        public int Id { get; set; }
        [Required, MinLength(5), MaxLength(10)]public string CustomerType { get; set; }
        [Required, MinLength(5), MaxLength(50)]public string CustomerName { get; set; }
        [Required, MinLength(4), MaxLength(20)]public string KnownAs { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Website { get; set; }
        public string Email {get; set;}
        public string Add { get; set; }
        public string Add2 { get; set; }
        [Required, MinLength(5), MaxLength(25)]public string City { get; set; }
        public string Pin { get; set; }
        public string District {get; set;}
        public string State { get; set; }
        public string Country { get; set; }="India";
        public string IntroducedBy { get; set; }
        public bool IsActive { get; set; }=true;
        public ICollection<CustomerIndustry> CustomerIndustries { get; set; }
        public ICollection<AgencySpecialty> AgencySpecialties {get; set;}
        public virtual ICollection<CustomerOfficialToCreateDto> CustomerOfficials {get; set;}
    }
}