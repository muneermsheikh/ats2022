using System;
using System.Threading.Tasks;
using core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class SMSService : ISMSService
     {
          private readonly IConfiguration _config;

          public SMSService(IConfiguration config)
          {
               _config = config;

          }

          public string sendMessage(string phoneno, string templateId)
          {
               var SMSKey = _config.GetSection("SMSSettings")["KeyNumber"];
               return "success";
          }
     }
}