using core.Entities.HR;
using Microsoft.AspNetCore.Http;

namespace core.Entities.Attachments
{
    public class FileAttachment
    {
        public IFormFile FormFile { get; set; }
         public EnumAttachmentType AttachmentType { get; set; }
    }
}