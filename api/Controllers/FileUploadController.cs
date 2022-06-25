using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using core.Entities.Admin;
using core.Entities.Attachments;
using core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
     public class FileUploadController : BaseApiController
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly UserManager<AppUser> _userManager;
        const string FILE_PATH = @"D:\UploadedFiles\";
        
          public FileUploadController(ILogger<FileUploadController> logger, UserManager<AppUser> userManager)
          {
            _logger = logger;
            _userManager = userManager;
          }

        [HttpGet("downloadcandidatefile/{candidateid:int}")]
        public async Task<ActionResult> DownloadFile(int candidateid)
        {
            
            //var filePath = $"{candidateid}.txt"; // Here, you should validate the request and the existance of the file.
            //DirectoryInfo source = new DirectoryInfo(SourceDirectory);

            var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //if (!File.Exists(FileName)) return false;

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            return File(bytes, contentType, Path.GetFileName(FileName));
        }

        [Authorize]
        [HttpGet("downloadprospectivefile/{prospectiveid:int}")]
        public async Task<ActionResult> DownloadProspectiveFile(int prospectiveid)
        {
            
            //var filePath = $"{candidateid}.txt"; // Here, you should validate the request and the existance of the file.
            //DirectoryInfo source = new DirectoryInfo(SourceDirectory);

            var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //if (!File.Exists(FileName)) return false;

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            return File(bytes, contentType, Path.GetFileName(FileName));
        }

        
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromBody]ICollection<FileToUpload> theFiles) 
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email==null) return BadRequest("User email not found");
            var user = await _userManager.FindByEmailAsync(email);
            if (user==null) return BadRequest("User Claim not found");
            var username = user.UserName;

            foreach(var theFile in theFiles)
            {
                var filePathName = FILE_PATH + Path.GetFileNameWithoutExtension(theFile.FileName) + "-" +
                DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                username +
                Path.GetExtension(theFile.FileName);
                
                if (theFile.FileAsBase64.Contains(","))     //the file begins with a header of file type followed by a comma, strip off this header from the filename
                {
                    theFile.FileAsBase64 = theFile.FileAsBase64.Substring(theFile.FileAsBase64.IndexOf(",") + 1);
                }

                //the file of type FileAsBase64 is a base64 encoded file.  to be readable by systems, it needs to be converted from base64string
                theFile.FileAsByteArray = Convert.FromBase64String(theFile.FileAsBase64);
                
                //create a new object and pass the byte array to the method of this method. 
                //Pass in a zero as the second parameter and the length of the byte array as the third parameter so the complete file is written to disk
                
                using (var fs = new FileStream(filePathName, FileMode.CreateNew)) 
                {
                    fs.Write(theFile.FileAsByteArray, 0, theFile.FileAsByteArray.Length);
                }
            }

            return Ok();
        }

       [HttpGet("LoadDocument")]
    
        public LoadedDocument LoadDocument()
        {
            string documentName = "invoice.docx";
            LoadedDocument document = new LoadedDocument()
            {
            DocumentData = Convert.ToBase64String(
                System.IO.File.ReadAllBytes("App_Data/" + documentName)),
            DocumentName = documentName
            };

            return document;
        }
    }
}