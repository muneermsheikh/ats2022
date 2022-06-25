using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using core.Entities.Attachments;
using infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class AttachmentController : BaseApiController
     {
          private readonly ATSContext _context;
          public AttachmentController(ATSContext context)
          {
               _context = context;
          }

          [HttpPost]
          public async Task<IActionResult> UploadToFileSystem(List<IFormFile> files, string description)
          {
               foreach (var file in files)
               {
                    var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
                    bool basePathExists = System.IO.Directory.Exists(basePath);
                    if (!basePathExists) Directory.CreateDirectory(basePath);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var filePath = Path.Combine(basePath, file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    if (!System.IO.File.Exists(filePath))
                    {
                         using (var stream = new FileStream(filePath, FileMode.Create))
                         {
                              await file.CopyToAsync(stream);
                         }
                         var fileModel = new FileOnFileSystem
                         {
                              CreatedOn = DateTime.UtcNow,
                              FileType = file.ContentType,
                              Extension = extension,
                              Name = fileName,
                              Description = description,
                              FilePath = filePath
                         };
                         _context.FilesOnFileSystem.Add(fileModel);
                         _context.SaveChanges();
                    }
               }
               return null;
          }

     }
}