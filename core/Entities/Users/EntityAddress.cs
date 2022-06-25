using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Users
{
    public class EntityAddress: BaseEntity
    {
          public EntityAddress()
          {
          }

          public EntityAddress(string addressType, string add, string streetAdd, string city, 
            string pin, string state, string district, string country, bool isMain)
          {
               AddressType = addressType;
               Add = add;
               StreetAdd = streetAdd;
               City = city;
               Pin = pin;
               State = state;
               District = district;
               Country = country;
               IsMain = isMain;
          }

        //public int CandidateId { get; set; }
        public string AddressType { get; set; }
        public string Add { get; set; }
        public string StreetAdd { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public bool IsMain { get; set; }
        public int CandidateId { get; set; }
        //public Candidate Candidate {get; set;}
    }
}