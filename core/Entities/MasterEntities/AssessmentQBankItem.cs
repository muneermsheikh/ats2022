using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.MasterEntities
{
    public class AssessmentQBankItem: BaseEntity
    {
        public int AssessmentQBankId { get; set; }
        public string AssessmentParameter { get; set; }
        public int QNo { get; set; }
        public bool IsStandardQ { get; set; }
        public string Question { get; set; }
        public int MaxPoints { get; set; }
    }
}