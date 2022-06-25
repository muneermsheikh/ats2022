using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Errors;
using api.Helpers;
using AutoMapper;
using core.Entities;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    //[Authorize(Policy = "Employee, CustomerMaintenanceRole")]
     public class CustomersController : BaseApiController
     {
          private readonly IGenericRepository<Customer> _custRepo;
          private readonly IMapper _mapper;
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICustomerService _customerService;
          private readonly UserManager<AppUser> _usermanager;
          public CustomersController(IGenericRepository<Customer> custRepo, IMapper mapper,
            IUnitOfWork unitOfWork, ICustomerService customerService, UserManager<AppUser> usermanager)
          {
               _usermanager = usermanager;
               _customerService = customerService;
               _unitOfWork = unitOfWork;
               _mapper = mapper;
               _custRepo = custRepo;
          }

        [Authorize(Roles="Admin, DocumentControllerAdmin, HRManager")]
        [HttpPost("registercustomers")]
        public async Task<ActionResult<ICollection<CustomerDto>>> RegisterCustomers(ICollection<RegisterCustomerDto> dtos)
        {
            var customers = await _customerService.AddCustomers(dtos);
            if(customers == null) return BadRequest(new ApiResponse(400, "failed to save the customers"));
            return Ok(customers);

        }

        [Authorize(Roles="Admin, DocumentControllerAdmin, HRManager")]
        [HttpPost("registercustomer")]
        public async Task<ActionResult<CustomerDto>> RegisterCustomer(RegisterCustomerDto dto)
        {
            return await _customerService.AddCustomer(dto);
        }

        [Authorize(Roles="Admin, DocumentControllerAdmin, HRManage, HRSupervisor")]
        [HttpGet]
        public async Task<ActionResult<Pagination<Customer>>> GetCustomers([FromQuery] CustomerSpecParams custParams)
        {
            if (custParams.CustomerCityName == "All") custParams.CustomerCityName="";
            var specs = new CustomerWithOfficialsSpecs(custParams);
            var countSpec = new CustomersWithFiltersForCountSpecs(custParams);
            var customers = await _unitOfWork.Repository<Customer>().ListAsync(specs);
            var totalCount = await _unitOfWork.Repository<Customer>().CountAsync(countSpec);

            return Ok(new Pagination<Customer>(custParams.PageIndex, custParams.PageSize, totalCount, customers));

        }

        [Authorize]
        [HttpGet("associateidandnames/{usertype}")]
        public async Task<ActionResult<ICollection<CustomerIdAndNameDto>>> GetCustomerIdAndNames(string usertype)
        {
            return Ok(await _customerService.GetCustomerIdAndName(usertype));
        }

        [Authorize]
        [HttpGet("byId/{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(int Id)
        {
            //var cust = await _unitOfWork.Repository<Customer>().GetByIdAsync(Id);
            var cust = await _customerService.GetCustomerByIdAsync(Id);
            var cus = _mapper.Map<Customer, CustomerDto>(cust);
            return Ok(cus);
        }

        [Authorize(Roles="Admin, DocumentControllerAdmin, HRManager")]
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateCustomer(Customer customer)
        {
            var edited = await _customerService.EditCustomer(customer);

            if (!edited) return NoContent();

            return Ok(true);
        }

        [Authorize]
        [HttpGet("officialidandname/{custType}")]
        public async Task<ActionResult<ICollection<CustomerOfficialDto>>> CustomerOfficialIdAndNames(string custType)
        {
            var users = await _customerService.GetCustomerIdAndName(custType);
            return Ok(users);
        }
        
        
        [Authorize(Roles="Admin, DocumentControllerAdmin, HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin, DocumentControllerProcess")]
        [HttpGet("agentdetails")]
        public async Task<ActionResult<ICollection<CustomerOfficialDto>>> GetCustomerOfficialIds()
        {
            var users = await _customerService.GetOfficialDetails();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("customerCities/{customerType}")]
        public async Task<ICollection<CustomerCity>> GetCustomerCities (string customerType)
        {
            return await _customerService.GetCustomerCityNames(customerType);
        }
        
        private async Task<ActionResult<bool>> CheckEmailExistsAsync(string email)
        {
            return await _usermanager.FindByEmailAsync(email) != null;
        }
    }
}