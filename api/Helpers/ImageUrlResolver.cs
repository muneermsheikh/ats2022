using api.DTOs;
using AutoMapper;
using core.Entities;
using Microsoft.Extensions.Configuration;

namespace api.Helpers
{
     public class ImageUrlResolver : IValueResolver<CustomerOfficial, CustomerOfficialToReturnDto, string>
     {
          private readonly IConfiguration _config;
          public ImageUrlResolver(IConfiguration config)
          {
               _config = config;
          }

          public string Resolve(CustomerOfficial source, CustomerOfficialToReturnDto destination, string destMember, ResolutionContext context)
          {
               if(!string.IsNullOrEmpty(source.ImageUrl)) {
                   return _config["ApiUrl"] + source.ImageUrl;
               }

               return null;
          }
     }
}