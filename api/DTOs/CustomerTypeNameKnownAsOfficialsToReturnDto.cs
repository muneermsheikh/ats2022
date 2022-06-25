using System.Collections.Generic;
using core.Entities;

namespace api.DTOs
{
    public class CustomerTypeNameKnownAsOfficialsToReturnDto
    {
        public int Id { get; set; }
        public string CustmerType { get; set; }
        public string CustomerName { get; set; }
        public string KnownAs { get; set; }
        public ICollection<CustomerOfficial> CustomerOfficials { get; set; }
    }
}