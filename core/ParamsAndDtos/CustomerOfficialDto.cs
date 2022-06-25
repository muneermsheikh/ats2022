namespace core.ParamsAndDtos
{
    public class CustomerOfficialDto
    {
        public CustomerOfficialDto()
        {
        }

          public CustomerOfficialDto(int id, int customerId, string customerName, 
            string city, string title, string officialName, string designation, 
            string officialEmailId, string mobileNo)
          {
               OfficialId = id;
               CustomerId = customerId;
               CustomerName = customerName;
               City = city;
               Title = title;
               OfficialName = officialName;
               Designation = designation;
               OfficialEmailId = officialEmailId;
               MobileNo = mobileNo;
          }

        public int OfficialId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName {get; set;}
        public string City {get; set;}
        public string Title { get; set; }
        public string OfficialName { get; set; }
        public string Designation { get; set; }
        public string OfficialEmailId { get; set; }
        public string MobileNo { get; set; }
        public bool Checked {get; set;}
        public bool CheckedPhone {get; set;}
        public bool CheckedMobile {get; set;}

    }
}