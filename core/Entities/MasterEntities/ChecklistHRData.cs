using System.ComponentModel.DataAnnotations;

namespace core.Entities.MasterEntities
{
    public class ChecklistHRData: BaseEntity
    {
          public ChecklistHRData()
          {
          }

          public ChecklistHRData(int srno, string parameter, bool mandatory)
          {
               SrNo = srno;
               Parameter = parameter;
               MandatoryTrue=mandatory;
          }

        [Range(1,1000)]
        public int SrNo {get; set;}
        public string Parameter { get; set; }
        public bool MandatoryTrue {get; set;}
    }
}