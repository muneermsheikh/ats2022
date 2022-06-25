using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.MasterEntities
{
    public class AssessmentQBank: BaseEntity
    {
        public int CategoryId { get; set; }
        public string CategoryName {get; set;}
        public ICollection<AssessmentQBankItem> AssessmentQBankItems { get; set; }
    }
}