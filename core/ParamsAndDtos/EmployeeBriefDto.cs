using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class EmployeeBriefDto
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string SecondName {get; set;}
        public string FamilyName { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public string Department { get; set; }
        public int? EmployeeStatus { get; set; }
        public string Position { get; set; }
    }
}