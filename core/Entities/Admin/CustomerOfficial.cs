using core.Entities.Admin;

namespace core.Entities
{
    public class CustomerOfficial: BaseEntity
    {
          public CustomerOfficial()
          {
          }

          public CustomerOfficial(int customerId, string gender, string title, string officialName, string designation, 
            string phoneNo, string mobile, string email, string imageUrl, bool login, bool isValid)
          {
               CustomerId = customerId;
               Gender = gender;
               Title = title;
               OfficialName = officialName;
               Designation = designation;
               PhoneNo = phoneNo;
               Mobile = mobile;   //used as whatsApp
               Email = email;
               ImageUrl = imageUrl;
               IsValid = isValid;
               LogInCredential = login;
          }

        public CustomerOfficial(int appuserid, string gender, string title, string officialName, string designation, 
            string phoneNo, string mobile, string email, string imageUrl, bool login)
          {
               AppUserId = appuserid;
               Gender = gender;
               Title = title;
               OfficialName = officialName;
               Designation = designation;
               PhoneNo = phoneNo;
               Mobile = mobile;   //also used as whatsApp
               Email = email;
               ImageUrl = imageUrl;
               LogInCredential = login;
          }

        public bool LogInCredential {get; set;}=false;
        public int AppUserId { get; set; }
        public string Gender { get; set; }="M";
        public string Title { get; set; }
        public string OfficialName { get; set; }
        public string Designation { get; set; }
        public string Divn {get; set;}  
        public string PhoneNo { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public bool IsValid { get; set; }=true;
        public int CustomerId { get; set; }
        //public Customer Customer {get; set;}
    }
}