using System;

namespace core.Entities.HR
{
    public class ChecklistHRItem: BaseEntity
    {
        public ChecklistHRItem()
          {
          }

        public ChecklistHRItem(int srNo, string parameter)
          {
               SrNo = srNo;
               Parameter = parameter;
          }
          
          public ChecklistHRItem(int srNo, string parameter,  bool mandatory)
          {
               SrNo = srNo;
               Parameter = parameter;
               MandatoryTrue = mandatory;
          }
        public ChecklistHRItem(int id, int checklisthrid, bool accepts,  bool mandatorytrue, int srNo, string parameter, string response, string exceptions)
          {
               Id = id;
               ChecklistHRId = checklisthrid;
               Accepts = accepts;
               MandatoryTrue = mandatorytrue;
               SrNo = srNo;
               Parameter = parameter;
               Response = response;
               Exceptions = exceptions;
          }

        public int ChecklistHRId { get; set; }
        public int SrNo {get; set;}
        public string Parameter {get; set;}
        public bool Accepts {get; set;}=false;
        public string Response {get; set;}
        public string Exceptions {get; set;}
        public bool MandatoryTrue {get; set;}
        
        //public ChecklistHR ChecklistHR {get; set;}
    }
} 