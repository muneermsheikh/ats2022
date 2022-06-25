using System.Threading.Tasks;
using api.Errors;
using core.Entities;
using core.Params;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [ApiController]
     [Route("/api/[controller]")]
     public class BaseApiController : ControllerBase
     {
     }
}