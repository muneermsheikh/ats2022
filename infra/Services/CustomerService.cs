using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class CustomerService : ICustomerService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IMapper _mapper;
          private readonly UserManager<AppUser> _userManager;
          private readonly IUserService _userService;
          private readonly ATSContext _context;
          public CustomerService(IUnitOfWork unitOfWork, IMapper mapper,
               UserManager<AppUser> userManager, IUserService userService,
               ATSContext context)
          {
               _context = context;
               _userService = userService;
               _userManager = userManager;
               _mapper = mapper;
               _unitOfWork = unitOfWork;
          }

          public async Task<ICollection<CustomerDto>> AddCustomers(ICollection<RegisterCustomerDto> dtos)
          {
               var customers = new List<Customer>();
               //using (var scope = new TransactionScope()) 
               //{
                    foreach(var dto in dtos)
                    {
                         var custIndustries = new List<CustomerIndustry>();
                         var custOfficials = new List<CustomerOfficial>();
                         var agencySpecialties = new List<AgencySpecialty>();

                         if (dto.CustomerIndustries != null && dto.CustomerIndustries.Count > 0) 
                         {
                              foreach (var ind in dto.CustomerIndustries)
                              {
                                   custIndustries.Add(new CustomerIndustry { IndustryId = ind.IndustryId });
                              }
                              custIndustries = custIndustries.Count() > 0 ? custIndustries : null;
                         }

                         if (dto.CustomerOfficials != null && dto.CustomerOfficials.Count >0)
                         {
                              //UserManager.CreateAsync never fails, so it will be added after customer official is added succesfully
                              //create identity users for each customer official
                              foreach (var off in dto.CustomerOfficials)
                              {
                                   custOfficials.Add(new CustomerOfficial(off.AppUserId, off.Gender, off.Title,
                                        off.FirstName + " " + off.SecondName + " " + off.FamilyName, off.Designation,
                                             off.PhoneNo, off.Mobile, off.Email, off.ImageUrl, off.LogInCredential));
                              }
                              custOfficials = custOfficials.Count() > 0 ? custOfficials : null;
                         }
                         
                         if (dto.AgencySpecialties != null && dto.AgencySpecialties.Count > 0 )
                         {
                              if(dto.AgencySpecialties!=null && dto.AgencySpecialties.Count() > 0)
                              {
                                   foreach (var sp in dto.AgencySpecialties)
                                   {
                                        agencySpecialties.Add(new AgencySpecialty { IndustryId = sp.IndustryId, ProfessionId = sp.ProfessionId });
                                   }
                              }
                              agencySpecialties = agencySpecialties.Count() > 0 ? agencySpecialties : null;
                         }
                         
                         //add the customer
                         var customer = new Customer(dto.CustomerType, dto.CustomerName, dto.KnownAs, dto.Add,
                              dto.Add2, dto.City, dto.Pin, dto.District, dto.State, dto.Country, dto.Email,
                              dto.Website, dto.Phone, dto.Phone2, custIndustries, custOfficials, agencySpecialties);

                         _unitOfWork.Repository<Customer>().Add(customer);
                         customers.Add(customer);
                    }
                    
                    var result = await _unitOfWork.Complete();

                    //now create identity users for each customer official, and update customer official table
                    //this could have been done before adding the customer official, but if Usermanager.CreateAsync succeeds
                    //(which always succeeds) and customer official insert fails, then we are left with user identity without
                    //corresponding customer officials.  So next time the customer official is to be added, it will nto succeed
                    //because its email Id already exists in AppUser
                    if (result > 0) {
                         foreach(var customer in customers)
                         {
                              foreach(var off in customer.CustomerOfficials)
                              {
                                   if (!string.IsNullOrEmpty(off.Email) && !CheckEmailExistsAsync(off.Email).Result) {
                                        if (off.LogInCredential) 
                                        {
                                              var dtoCust = dtos.Where(x => x.CustomerName.ToLower() == customer.CustomerName.ToLower() && x.City.ToLower() == customer.City.ToLower()).FirstOrDefault();
                                             var dtoOff = dtoCust.CustomerOfficials.Where(x => 
                                                  x.FirstName.ToLower() + " " + x.SecondName.ToLower() + " " + x.FamilyName.ToLower() 
                                                  == off.OfficialName.ToLower())
                                                  .Select(y => new {y.KnownAs, y.Password})
                                                  .FirstOrDefault();
                                             
                                             var appuser = await CreateIdentityUser(off, dtoOff.Password);
                                             
                                             if (appuser != null) {
                                                  off.AppUserId = appuser.Id;
                                                  _unitOfWork.Repository<CustomerOfficial>().Update(off);
                                             }
                                        }
                                   }
                              }
                         }

                         await _unitOfWork.Complete();
                         return _mapper.Map<ICollection<Customer>, ICollection<CustomerDto>>(customers);
                    }
                    return null;
               //}
          }

          private async Task<AppUser> CreateIdentityUser(CustomerOfficial off, string password)
          {
                if (!off.LogInCredential) return null;
                    
               var appuser = new AppUser
               {
                    UserType = "official",
                    Gender = off.Gender,
                    DisplayName = off.OfficialName,
                    Email = off.Email,
                    UserName = off.Email,
                    PhoneNumber = off.PhoneNo
               };

               var added = await _userManager.CreateAsync(appuser, password);

               if (added.Succeeded) return appuser;

               return null;
                    
          }
          public async Task<CustomerDto> AddCustomer(RegisterCustomerDto dto)
          {
               var custIndustries = new List<CustomerIndustry>();
               foreach (var ind in dto.CustomerIndustries)
               {
                    custIndustries.Add(new CustomerIndustry { IndustryId = ind.IndustryId });
               }
               custIndustries = custIndustries.Count() > 0 ? custIndustries : null;

               var custOfficials = new List<CustomerOfficial>();
               foreach (var off in dto.CustomerOfficials)
               {
                    if (off.LogInCredential)
                    {
                         var official = _mapper.Map<CustomerOfficialToCreateDto, CustomerOfficial>(off);
                         var appuser = await CreateIdentityUser(official, off.Password);
                         if(appuser != null) off.AppUserId=appuser.Id;
                    }

                    custOfficials.Add(new CustomerOfficial(off.AppUserId, off.Gender, off.Title,
                         off.FirstName + " " + off.SecondName + " " + off.FamilyName, off.Designation,
                              off.PhoneNo, off.Mobile, off.Email, off.ImageUrl, off.LogInCredential));
               }

               custOfficials = custOfficials.Count() > 0 ? custOfficials : null;

               var agencySpecialties = new List<AgencySpecialty>();
               if(dto.AgencySpecialties!=null && dto.AgencySpecialties.Count() > 0)
               {
                    foreach (var sp in dto.AgencySpecialties)
                    {
                         agencySpecialties.Add(new AgencySpecialty { IndustryId = sp.IndustryId, ProfessionId = sp.ProfessionId });
                    }
               }
               agencySpecialties = agencySpecialties.Count() > 0 ? agencySpecialties : null;

               var customer = new Customer(dto.CustomerType, dto.CustomerName, dto.KnownAs, dto.Add,
                    dto.Add2, dto.City, dto.Pin, dto.District, dto.State, dto.Country, dto.Email,
                    dto.Website, dto.Phone, dto.Phone2, custIndustries, custOfficials, agencySpecialties);

               _unitOfWork.Repository<Customer>().Add(customer);

               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;
/*
               if (result > 0 && custOfficials !=null) {
                    foreach(var off in custOfficials) {
                         off.CustomerId = customer.Id;
                         _unitOfWork.Repository<CustomerOfficial>().Add(off);
                    }
                    result = await _unitOfWork.Complete();
               }
*/

               return _mapper.Map<Customer, CustomerDto>(
                    customer /*await _context.Customers.Where(x => x.Id == customer.Id).
                    Include(x => x.CustomerOfficials).
                    Include(X => X.CustomerIndustries).
                    Include(X => X.AgencySpecialties).FirstOrDefaultAsync()*/
               );
          }

          public Task<bool> CustomerExistsByIdAsync(int id)
          {
               throw new System.NotImplementedException();
          }

          public void DeleteCustomer(Customer customer)
          {
               throw new System.NotImplementedException();
          }

          public async Task<bool> EditCustomer(Customer model)
          {
               string Password="Pa$$w0rd";

               var existingCustomer = await _context.Customers
                    .Where(x => x.Id == model.Id)
                    .Include(x => x.CustomerOfficials)
                    .Include(x => x.AgencySpecialties)
                    .Include(x => x.CustomerIndustries)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
               

               if (existingCustomer==null) return false;

               //update the top level object
               _context.Entry(existingCustomer).CurrentValues.SetValues(model);

               //work on navigation objects

               //delete records that exist in DB but not in model
               //officials
               foreach(var existingOfficial in existingCustomer.CustomerOfficials.ToList() )
               {
                    if (!model.CustomerOfficials.Any(c => c.Id == existingOfficial.Id && c.Id != default(int)))
                    {
                         _context.CustomerOfficials.Remove(existingOfficial);
                         _context.Entry(existingOfficial).State = EntityState.Deleted;
                    }
               }

               //customerindustries
               foreach(var existingInd in existingCustomer.CustomerIndustries.ToList() )
               {
                    if (!model.CustomerIndustries.Any(c => c.Id == existingInd.Id && c.Id != default(int)))
                    {
                         _context.CustomerIndustries.Remove(existingInd);
                         _context.Entry(existingInd).State = EntityState.Deleted;
                    }
               }

               //delete agencyspecialties
               foreach(var existingSpecialty in existingCustomer.AgencySpecialties.ToList() )
               {
                    if (!model.AgencySpecialties.Any(c => c.Id == existingSpecialty.Id && c.Id != default(int)))
                    {
                         _context.AgencySpecialties.Remove(existingSpecialty);
                         _context.Entry(existingSpecialty).State = EntityState.Deleted;
                    }
               }

               //update/add records in model navigation objects

               //officials
               foreach(var off in model.CustomerOfficials)
               {
                    var existingOff = existingCustomer.CustomerOfficials.Where(c => c.Id == off.Id && c.Id != default(int)).SingleOrDefault();
                    if(existingOff != null)  //update
                    {
                         _context.Entry(existingOff).CurrentValues.SetValues(off);
                         _context.Entry(existingOff).State = EntityState.Modified;
                    } else //added
                    {
                         var appuser = await CreateIdentityUser(off, Password);
                         var newOff = new CustomerOfficial(appuser==null ? 0 : appuser.Id, off.Gender, off.Title, off.OfficialName,
                              off.Designation, off.PhoneNo, off.Mobile, off.Email, off.ImageUrl, off.LogInCredential,true);
                         existingCustomer.CustomerOfficials.Add(newOff);
                         _context.Entry(newOff).State=EntityState.Added;
                    }
               }


               //industries
               foreach(var ind in model.CustomerIndustries)
               {
                    var existingInd = existingCustomer.CustomerIndustries.Where(c => c.Id == ind.Id && c.Id != default(int)).SingleOrDefault();
                    if(existingInd != null)  //update
                    {
                         _context.Entry(existingInd).CurrentValues.SetValues(ind);
                         _context.Entry(existingInd).State = EntityState.Modified;
                    } else //added
                    {
                         var newInd = new CustomerIndustry{CustomerId=model.Id, IndustryId = ind.IndustryId, Name = ind.Name};
                         existingCustomer.CustomerIndustries.Add(newInd);
                         _context.Entry(newInd).State=EntityState.Added;
                    }
               }

               
               //agencyspecialties
               foreach(var sp in model.AgencySpecialties)
               {
                    var existingSp = existingCustomer.AgencySpecialties.Where(c => c.Id == sp.Id && c.Id != default(int)).SingleOrDefault();
                    if(existingSp != null)  //update
                    {
                         _context.Entry(existingSp).CurrentValues.SetValues(sp);
                         _context.Entry(existingSp).State = EntityState.Modified;
                    } else //added
                    {
                         var newSp = new AgencySpecialty{CustomerId=model.Id, ProfessionId=sp.ProfessionId, Name=sp.Name};
                         
                         existingCustomer.AgencySpecialties.Add(newSp);
                         _context.Entry(newSp).State=EntityState.Added;
                    }
               }

               _context.Entry(existingCustomer).State = EntityState.Modified;

               return await _context.SaveChangesAsync() > 0;
          }

          public async Task<Customer> GetCustomerByIdAsync(int id)
          {
               //var indChanged = false;
               var c = await _context.Customers 
                    .Where(x => x.Id == id)
                    .Include(x => x.CustomerOfficials)
                    .Include(x => x.CustomerIndustries)
                    .Include(x => x.AgencySpecialties)
                    .FirstOrDefaultAsync();
               /* foreach(var item in c.CustomerIndustries) {
                    if (string.IsNullOrEmpty(item.Name)) {
                         item.Name = await _context.Industries.Where(x => x.Id == item.IndustryId).Select(x => x.Name).FirstOrDefaultAsync();
                         _unitOfWork.Repository<CustomerIndustry>().Update(item);
                         indChanged=true;
                    }
               }
               if (indChanged) {
                    await _unitOfWork.Complete();
               }
               */
               return c;
          }

          public Task<CustomerDto> GetCustomerByUserNameAsync(string username)
          {
               throw new System.NotImplementedException();
          }

          public async Task<ICollection<CustomerIdAndNameDto>> GetCustomerIdAndName(string customerType)
          {
               var qry = await _context.Customers
                    .Where(x => x.CustomerType.ToLower() == customerType)
                    .Select(x => new {x.Id, x.CustomerName, x.City})
                    .OrderBy(x => x.CustomerName)
                    .ToListAsync();
               var lst = new List<CustomerIdAndNameDto>();
               foreach(var item in qry)
               {
                    lst.Add(new CustomerIdAndNameDto{Id=item.Id, CustomerName=item.CustomerName, City=item.City});
               }
               return lst;
          }

          public Task<ICollection<CustomerIdAndNameDto>> GetCustomerIdAndNames(ICollection<int> customerIds)
          {
               throw new System.NotImplementedException();
          }

          public Task<string> GetCustomerNameFromId(int Id)
          {
               throw new System.NotImplementedException();
          }

          public Task<ICollection<CustomerDto>> GetCustomersAsync(string userType)
          {
               throw new System.NotImplementedException();
          }

          public Task<ICollection<CustomerDto>> GetCustomersPaginatedAsync(CustomerParams custParam)
          {
               throw new System.NotImplementedException();
          }

          public void UpdateCustomer(Customer customer)
          {
               throw new System.NotImplementedException();
          }
          private async Task<bool> CheckEmailExistsAsync(string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }

          public async Task<ICollection<CustomerCity>> GetCustomerCityNames(string customerType)
          {
               var c = await _context.Customers.Where(x => x.CustomerType.ToLower() == customerType.ToLower())
                    .Select(x => x.City).Distinct().ToListAsync();
               var lsts = new List<CustomerCity>();
               foreach(var lst in c)
               {
                    lsts.Add(new CustomerCity{CityName = lst});
               }
               return lsts;
          }

          public Task<ICollection<string>> GetCustomerIndustryTypes(string customerType)
          {
               throw new NotImplementedException();
          }

          public async Task<ICollection<CustomerOfficialDto>> GetOfficialDetails()
          {
               var offs = await (from c in _context.Customers
                         .Where(x => x.CustomerType == "associate" && x.CustomerStatus==EnumCustomerStatus.Active )
                         join o in _context.CustomerOfficials on c.Id equals o.CustomerId where o.IsValid
                         select new CustomerOfficialDto(o.Id, c.Id,  c.CustomerName, c.City, o.Title, 
                              o.OfficialName, o.Designation, o.Email, o.Mobile)
                    ).ToListAsync();
               
               return offs;
          }
     }
}