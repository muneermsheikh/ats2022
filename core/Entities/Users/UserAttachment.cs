using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Users
{
    public class UserAttachment: BaseEntity
    {
        public UserAttachment()
        {
        }

        public UserAttachment(int appUserId, string fileName)
        {
            AppUserId = appUserId;
            //AttachmentType = attachmentType;
            //AttachmentSizeInKB = attachmentSizeInKB;
            //AttachmentUrl = attachmentUrl;

        }

        public int CandidateId { get; set; }
        public int AppUserId { get; set; }
        public string AttachmentType { get; set; }      //cv, photo, educertificate, expcertificate, pp
        public long AttachmentSizeInBytes { get; set; }
        //public string AttachmentUrl {get; set;}
        //public Candidate Candidate {get; set;}
        public string url  { get; set; }
        public DateTime DateUploaded {get; set;}
        public int UploadedByEmployeeId {get; set;} 
        
    }
}