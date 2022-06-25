using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.HR
{
    public class AssessmentStandardQ: BaseEntity
    {
        public string AssessmentParameter { get; set; }
        public int QNo { get; set; }
        public string Question { get; set; }
        public int MaxPoints { get; set; }
    }
}