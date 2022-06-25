using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class ContactResult: BaseEntity
    {
        public string Name { get; set; }
        public int EnumNo {get; set;}
        public string PersonType {get; set;}
    }
}