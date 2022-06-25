namespace core.Entities.Users
{
    public class EmployeePhone: BaseEntity
    {
        public EmployeePhone()
        {
        }
        public EmployeePhone(string mobileNo, bool isMain)
        {
            MobileNo = mobileNo;
            IsMain = isMain;
        }
        
        public EmployeePhone(int employeeId,  string mobileNo, bool isOfficial, bool isMain)
        {
            EmployeeId = employeeId;
            MobileNo = mobileNo;
            IsMain = isMain;
            IsOfficial = isOfficial;
        }

        public int EmployeeId { get; set; }
        public string MobileNo { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsMain {get; set;}=false;
        public bool IsValid { get; set; }=true;
    }
}