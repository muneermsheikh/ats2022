using System;
using core.Entities.HR;

namespace core.Entities.Attachments
{
    public class FileUpload: BaseEntity
    {
          public FileUpload(int appUserId, string attachmentType, long length, string name, 
            int uploadedbyUserId, DateTime uploadedOn, bool isCurrent)
          {
               AppUserId = appUserId;
               AttachmentType = attachmentType;
               Length = length;
               Name = name;
               UploadedbyUserId = uploadedbyUserId;
               UploadedOn = uploadedOn;
               IsCurrent = isCurrent;
          }

        public int AppUserId { get; set; }
        //public EnumAttachmentType AttachmentType { get; set; }
        public string AttachmentType { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public string UploadedLocation { get; set; }
        public int UploadedbyUserId { get; set; }
        public DateTime UploadedOn { get; set; }
        public bool IsCurrent { get; set; }=true;
    }
}