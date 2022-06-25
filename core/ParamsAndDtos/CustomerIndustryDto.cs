using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class CustomerIndustryDto
    {
        public int CustomerId { get; set; }
        public int IndustryId { get; set; }
        public string Name { get; set; }
    }
}