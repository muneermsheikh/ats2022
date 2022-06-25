using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.ParamsAndDtos
{
    public class ChooseAgentDto
    {
        public ChooseAgentDto()
        {
        }

        public ChooseAgentDto(int customerid, string customerName, int officialId, string officialName, 
            string city, string designation, string title, string email, string phoneno, string mobile)
        {
            CustomerId = customerid;
            CustomerName = customerName;
            OfficialId = officialId;
            OfficialName = officialName;
            City = city;
            Designation = designation;
            Title = title;
            Email = email;
            PhoneNo = phoneno;
            Mobile = mobile;
        }

        public int CustomerId {get; set;}
        public string CustomerName { get; set; }
        public int OfficialId { get; set; }
        public string City { get; set; }
        public string OfficialName { get; set; }
        public string Title { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string PhoneNo {get; set;}
        public string Mobile {get; set;}
        public int Value { get; set; }
        public bool Checked { get; set; }=false;
        public bool CheckedPhone {get; set;}=false;
        public bool CheckedWhatsApp {get; set;}=false;
        public bool IsBlocked { get; set; }=false;
    }
}