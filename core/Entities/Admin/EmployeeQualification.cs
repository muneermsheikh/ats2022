namespace core.Entities.Admin
{
    public class EmployeeQualification: BaseEntity
    {
        public EmployeeQualification()
        {
        }

        public EmployeeQualification(int qualificationId, bool isMain)
        {
            QualificationId = qualificationId;
            IsMain = isMain;
        }

        public EmployeeQualification(int employeeId, int qualificationId, bool isMain)
        {
            EmployeeId = employeeId;
            QualificationId = qualificationId;
            IsMain = isMain;
        }

        public int EmployeeId { get; set; }
        public int QualificationId { get; set; }
        public bool IsMain { get; set; }
        //public Employee Employee {get; set;}
    }
}