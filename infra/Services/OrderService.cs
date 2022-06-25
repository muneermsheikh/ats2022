using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class OrderService : IOrderService
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly UserManager<AppUser> _userManager;
          //private readonly IPaymentService _paymentService;
          private readonly IGenericRepository<OrderItem> _orderItemRepo;
          private readonly IMapper _mapper;
          private readonly IComposeMessagesForAdmin _composeMessages;
          private readonly ITaskService _taskService;
          private readonly ICommonServices _commonServices;
          private readonly IConfiguration _config;
          public OrderService(IUnitOfWork unitOfWork, IComposeMessagesForAdmin composeMessages
               , ATSContext context, UserManager<AppUser> userManager, ICommonServices commonServices
               , IGenericRepository<OrderItem> orderItemRepo, IMapper mapper, ITaskService taskService
               , IConfiguration config)
          {
               _taskService = taskService;
               _context = context;
               _commonServices = commonServices;
          
               //_paymentService = paymentService;
               _userManager = userManager;
               _unitOfWork = unitOfWork;
               _orderItemRepo = orderItemRepo;
               _mapper = mapper;
               _composeMessages = composeMessages;
               _config = config;
          }


          public async Task<ICollection<Order>> CreateOrdersAsync(int loggedInUserId, ICollection<OrderToCreateDto> dtos)
          {
               string salesmanName = "";
               var orders = new List<Order>();
               var orderNo = await _context.Orders.MaxAsync(x => (int?)x.OrderNo) ?? 1000;
               var defaultProjectManagerId = _config.GetConnectionString("DefaultProjectManagerId");
               var defaultVisaExecutiveId = _config.GetConnectionString("DefaultVisaProcessInchargeId_KSA");
               var defaultMedicalInchargeId_KSA = _config.GetConnectionString("DefaultMedicalInchargeId_KSA");
               var defaultTravellingInchargeId = _config.GetConnectionString("DefaultTravellingInchargeId");

               foreach(var dto in dtos)
               {
                    ++orderNo;
                    if (dto.SalesmanId != 0) salesmanName = await EmployeeNameEmployeeId((int)dto.SalesmanId);

                    //isnert customer name
                    var cus = await _context.Customers.Where(x => x.Id == dto.CustomerId)
                         .Select(x => new { x.CustomerName, x.Add, x.Add2, x.City, x.Pin, x.District, x.State, x.Country, x.Email })
                         .FirstOrDefaultAsync();
                    if (cus == null) continue;
                    
                    dto.CustomerName = cus.CustomerName;

                    var subtotal = 0;
                    var items = new List<OrderItem>();
                    foreach (var item in dto.OrderItems)
                    {
                         item.JobDescription.OrderNo = orderNo;
                         item.Remuneration.OrderNo = orderNo;
                         var categoryName = await CategoryNameFromId(item.CategoryId);
                         var industryName = await IndustryNameFromId(item.IndustryId);
                         items.Add(new OrderItem(item.SrNo, orderNo, item.CategoryId, categoryName, item.IndustryId,
                              industryName, item.SourceFrom, item.Quantity, item.MinCVs, item.MaxCVs,
                              item.Ecnr, item.RequireAssess, item.CompleteBefore, item.Charges, item.JobDescription, item.Remuneration));
                              subtotal = items.Sum(item => item.Charges * item.Quantity) + (items.Sum(item => item.FeeFromClientINR * item.Quantity));
                    }

                    // create order

                    var order = new Order(orderNo, dto.CustomerId, dto.CustomerName, dto.CityOfEmployment, dto.OrderRef,
                         dto.OrderRefDate, (int)dto.SalesmanId, subtotal, dto.CompleteBy, items);
                    order.SalesmanName = salesmanName;
                    order.CityOfWorking = cus.City;
                    order.Country = cus.Country;
                    order.BuyerEmail = cus.Email ?? "not available";
                    order.ProjectManagerId = dto.ProjectManagerId.HasValue && dto.ProjectManagerId != 0 ? Convert.ToInt32(dto.ProjectManagerId) : Convert.ToInt32(defaultProjectManagerId);
                    order.VisaProcessInchargeEmpId = dto.VisaInchargeId.HasValue && dto.VisaInchargeId != 0 ? Convert.ToInt32(dto.VisaInchargeId) : Convert.ToInt32(defaultVisaExecutiveId);
                    order.TravelProcessInchargeId = dto.TravelInchargeId.HasValue && dto.TravelInchargeId != 0 ? Convert.ToInt32(dto.TravelInchargeId) : Convert.ToInt32(defaultTravellingInchargeId);

                    _unitOfWork.Repository<Order>().Add(order);
                    orders.Add(order);
               }

               var result = await _unitOfWork.Complete();
               if (result <= 0) return null;

               //update orderId and roderItemId in remunerations and JobDescription
               foreach(var order in orders)
               {
                    var orderid = order.Id;
                    foreach(var orderitem in order.OrderItems)
                    {
                         var orderItemId = orderitem.Id;
                         orderitem.JobDescription.OrderItemId=orderitem.Id;
                         orderitem.JobDescription.OrderId = orderid;
                         _unitOfWork.Repository<JobDescription>().Update(orderitem.JobDescription);

                         orderitem.Remuneration.OrderId = orderid;
                         orderitem.Remuneration.OrderItemId = orderitem.Id;
                         _unitOfWork.Repository<Remuneration>().Update(orderitem.Remuneration);                    
                    }
               }
               await _unitOfWork.Complete();

               foreach(var order in orders)
               {
                    await _composeMessages.AckEnquiryToCustomer(new OrderMessageParamDto { Order = order, DirectlySendMessage = false });
               }
               
               //create task for Admn Manager for contract review
               // return order
               return orders;
          }

          public async Task<Order> CreateOrderAsync(OrderToCreateDto dto)
          {
               string salesmanName = "";
               if (dto.SalesmanId != 0) salesmanName = await EmployeeNameEmployeeId((int)dto.SalesmanId);

               var orderNo = await _context.Orders.MaxAsync(x => (int?)x.OrderNo) ?? 1000;
               ++orderNo;

               //isnert customer name
               var cus = await _context.Customers.Where(x => x.Id == dto.CustomerId)
                    .Select(x => new { x.CustomerName, x.Add, x.Add2, x.City, x.Pin, x.District, x.State, x.Country, x.Email })
                    .FirstOrDefaultAsync();
               dto.CustomerName = cus.CustomerName;

               var subtotal = 0;
               var items = new List<OrderItem>();
               foreach (var item in dto.OrderItems)
               {
                    var categoryName = await CategoryNameFromId(item.CategoryId);
                    var industryName = await IndustryNameFromId(item.IndustryId);
                    items.Add(new OrderItem(item.SrNo, orderNo, item.CategoryId, categoryName, item.IndustryId,
                         industryName, item.SourceFrom, item.Quantity, item.MinCVs, item.MaxCVs,
                         item.Ecnr, item.RequireAssess, item.CompleteBefore, item.Charges, item.JobDescription, item.Remuneration));
                    subtotal = items.Sum(item => item.Charges * item.Quantity) + (items.Sum(item => item.FeeFromClientINR * item.Quantity));
               }

               // create order

               var order = new Order(orderNo, dto.CustomerId, dto.CustomerName, dto.CityOfEmployment, dto.OrderRef,
                    dto.OrderRefDate, (int)dto.SalesmanId, subtotal, dto.CompleteBy, items);
               order.SalesmanName = salesmanName;
               order.CityOfWorking = cus.City;
               order.Country = cus.Country;
               order.BuyerEmail = cus.Email ?? "not available";
               _unitOfWork.Repository<Order>().Add(order);

               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               //issue acaknowledgement
               await _composeMessages.AckEnquiryToCustomer(new OrderMessageParamDto { Order = order, DirectlySendMessage = false });
               //create task for Admn Manager for contract review
               // return order
               return order;
          }

          public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
          {
               return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
          }

          public async Task<IReadOnlyList<Order>> GetOrdersByEmailAsync(string email)
          {
               var spec = new OrdersWithItemsAndOrderingSpecs(email);

               return await _unitOfWork.Repository<Order>().ListAsync(spec);
          }

          public async Task<Pagination<OrderBriefDto>> GetOrdersAllAsync(OrdersSpecParams orderParams)
          {
               var spec = new OrdersWithItemsAndOrderingSpecs(orderParams);
               var countSpec = new OrdersWithItemsAndOrderingForCountSpecs(orderParams);
               var totalItems = await _unitOfWork.Repository<Order>().CountAsync(countSpec);
               var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);

               //
               foreach(var dto in orders)
               {
                    //isnert customer name
                    if (string.IsNullOrEmpty(dto.CustomerName)) dto.CustomerName=await _commonServices.CustomerNameFromCustomerId(dto.CustomerId);
               } 
               var data = _mapper.Map<IReadOnlyList<OrderBriefDto>>(orders);
               foreach(var d in data) {
                    if (d.ContractReviewId !=0) {
                         var crv = from rvw in _context.ContractReviews where rvw.Id == d.ContractReviewId
                              join emp in _context.Employees on rvw.ReviewedBy equals emp.Id
                              select new {
                                   reviewedBy = emp.KnownAs, reviewedOn = rvw.ReviewedOn, reviewStatusId = rvw.RvwStatusId
                              };
                         var crvw = await crv.FirstOrDefaultAsync();
                              
                         if (crvw != null) {
                              d.ReviewedBy = crvw.reviewedBy;
                              d.ReviewedOn = crvw.reviewedOn;
                              d.ReviewStatus  = Enum.GetName(typeof(EnumReviewStatus), crvw.reviewStatusId);
                         }     
                    }
               }
               return new Pagination<OrderBriefDto>(orderParams.PageIndex, orderParams.PageSize, totalItems, data);

          }
          
          public async Task<Order> GetOrderByIdWithItemsJDRemunertionAsyc (int id)
          {
               return await _context.Orders.Where(x => x.Id == id)
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.JobDescription)
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Remuneration)
               .FirstOrDefaultAsync();
          }
          public async Task<Order> GetOrderByIdWithItemsAsyc (int id)
          {
               return await _context.Orders.Where(x => x.Id == id)
                    .Include(x => x.OrderItems)
               .FirstOrDefaultAsync();
          }
          public async Task<Pagination<OrderBriefDto>> GetOrdersBriefAllAsync(OrdersSpecParams orderParams)
          {
               var spec = new OrdersBriefSpecs(orderParams);
               var countSpec = new OrdersBriefForCountSpecs(orderParams);
               var totalItems = await _unitOfWork.Repository<Order>().CountAsync(countSpec);
               var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);

               var data = _mapper.Map<IReadOnlyList<OrderBriefDto>>(orders);

               return new Pagination<OrderBriefDto>(orderParams.PageIndex, orderParams.PageSize, totalItems, data);
          }


          public async Task<bool> EditOrder(Order order, int loggedInUserId)
          {
               //thanks to @slauma of stackoverflow
               var existingOrder = _context.Orders.Where(p => p.Id == order.Id)
                    .Include(p => p.OrderItems).ThenInclude(p => p.JobDescription)
                    .Include(p => p.OrderItems).ThenInclude(p => p.Remuneration)
                    .AsNoTracking()
                    .SingleOrDefault();

               //check for foreign keys missing
               //if (order.SalesmanId == 0) order.SalesmanId = defaultSalesmanId;
               //if (order.ProjectManagerId == 0) order.ProjectManagerId = defaultProjectManagerId;
               if (existingOrder == null) return false;
               
               _context.Entry(existingOrder).CurrentValues.SetValues(order);   //saves only the parent, not children

               //the children - order items
               //Delete children that exist in existing record, but not in the new model order
               foreach (var existingItem in existingOrder.OrderItems.ToList())
               {
                    if (!order.OrderItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.OrderItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach (var item in order.OrderItems)
               {
                    var existingItem = existingOrder.OrderItems.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)       // Update child
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(item);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    }
                    else            //insert children as new record
                    {
                         var jd = item.JobDescription;
                         if (jd == null) jd = new JobDescription();
                         var newJd = new JobDescription(jd.JobDescInBrief ?? "Not Available",
                              jd.QualificationDesired ?? "Not Available",
                              jd.ExpDesiredMin, jd.ExpDesiredMax, jd.MinAge, jd.MaxAge);
                         var rem = item.Remuneration;
                         if (rem == null) rem = new Remuneration();
                         var newRem = new Remuneration(rem.SalaryCurrency ?? "???", rem.SalaryMin, rem.SalaryMax,
                              rem.ContractPeriodInMonths, rem.HousingProvidedFree, rem.HousingAllowance,
                              rem.FoodProvidedFree, rem.FoodAllowance, rem.TransportProvidedFree,
                              rem.TransportAllowance, rem.OtherAllowance, rem.LeavePerYearInDays,
                              rem.LeaveAirfareEntitlementAfterMonths);

                         var newItem = new OrderItem
                         {
                              OrderId = existingOrder.Id,
                              SrNo = item.SrNo,
                              CategoryId = item.CategoryId,
                              SourceFrom = item.SourceFrom,
                              Quantity = item.Quantity,
                              MinCVs = item.MinCVs,
                              MaxCVs = item.MaxCVs,
                              Ecnr = item.Ecnr,
                              RequireAssess = item.RequireAssess,
                              HrExecId = item.HrExecId,
                              HrSupId = item.HrSupId,
                              HrmId = item.HrmId,
                              AssignedId = item.AssignedId,
                              Charges = item.Charges,
                              Status = item.Status,
                              JobDescription = newJd,
                              Remuneration = newRem
                         };
                         existingOrder.OrderItems.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }
                    _context.Entry(existingOrder).State = EntityState.Modified;

               if (await _context.SaveChangesAsync() > 0)
               {
                    if (_context.Entry(order).State != EntityState.Unchanged) 
                    {      //record changed
                         var newTasks = new List<ApplicationTask>();
                    
                         foreach(var item in order.OrderItems) 
                         {
                              if (_context.Entry(item).State != EntityState.Unchanged) 
                              {
                                   //Tasks has unique index: OrerItemId + assignedToId + taskTypeId.  If the task exists, edit it, else add a new task
                                   var existingTask = await _context.Tasks.Where(x => 
                                        x.OrderItemId == item.Id && x.AssignedToId == item.HrExecId && x.TaskTypeId == (int)EnumTaskType.OrderEditedAdvise)
                                        .FirstOrDefaultAsync();
                                   if (existingTask == null) 
                                   {
                                        var newTask = new ApplicationTask((int)EnumTaskType.OrderEditedAdvise, DateTime.Now, 
                                             order.ProjectManagerId, order.ProjectManagerId, order.Id, order.OrderNo, item.Id, 
                                             "Order Item Changed Advise to Admin Dept and concerned HR Officials",
                                             DateTime.Now.AddDays(2), "Not Started", 0, null );
                                        _unitOfWork.Repository<ApplicationTask>().Add(newTask);
                                   } else 
                                   {
                                        existingTask.TaskDescription = existingTask.TaskDescription + " - order item changed on task completion date";
                                        _unitOfWork.Repository<ApplicationTask>().Update(existingTask);
                                        
                                        var newTaskItem = new TaskItem((int)EnumTaskType.OrderEditedAdvise, existingTask.Id, DateTime.Now,
                                        "started", "Order Item changed advise to admin and other HR Officials", order.Id,
                                             item.Id, order.OrderNo, loggedInUserId, DateTime.Now.AddDays(7), 0, 0, 0//, existingTask
                                        );
                                        _unitOfWork.Repository<TaskItem>().Add(newTaskItem);

                                        var newTask = new ApplicationTask((int)EnumTaskType.OrderEditedAdvise, DateTime.Now, 
                                             order.ProjectManagerId, order.ProjectManagerId, order.Id, order.OrderNo, item.Id, 
                                             "Order Item Changed Advise to Admin Dept and concerned HR Officials",
                                             DateTime.Now.AddDays(2), "Not Started", 0, null );
                                        _unitOfWork.Repository<ApplicationTask>().Add(newTask);
                                   }
                              }
                         }
                         
                         await _unitOfWork.Complete();
                         return true;
                    }
               } 

               return false;

          }

          public async Task<bool> DeleteOrder(Order order)
          {
               _context.Entry(order).State = EntityState.Deleted;
               return await _context.SaveChangesAsync() > 0;
          }
          private async Task<string> CategoryNameFromId(int id)
          {
               return await _context.Categories.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
          }

          private async Task<string> IndustryNameFromId(int id)
          {
               return await _context.Industries.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
          }

          private async Task<string> EmployeeNameEmployeeId(int id)
          {
               var nm = await _context.Employees.Where(x => x.Id == id)
                    .Select(x => new { x.FirstName, x.FamilyName }).FirstOrDefaultAsync();
               if (nm==null) return "";

               return nm.FirstName ?? "" + " " + nm.FamilyName ?? "";
          }

          public async Task<IReadOnlyList<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
          {
               var spec = new OrderItemSpecs(orderId);
               var items = await _orderItemRepo.ListAsync(spec);
               return items;
          }
          
          public async Task<OrderItem> GetOrderItemByOrderItemIdAsync(int Id)
          {
               var item = await _orderItemRepo.GetByIdAsync(Id);
               return item;
          }

          public void AddOrderItem(OrderItem orderItem)
          {
               var item = new OrderItem(orderItem.OrderId, orderItem.SrNo, orderItem.CategoryId, orderItem.CategoryName,
                    orderItem.IndustryId, orderItem.IndustryName, orderItem.SourceFrom, orderItem.Quantity, orderItem.MinCVs,
                    orderItem.MaxCVs, orderItem.Ecnr, orderItem.RequireAssess, orderItem.CompleteBefore, orderItem.Charges, false);

               _context.OrderItems.Add(item);
               _context.Entry(item).State = EntityState.Added;
          }

          public void EditOrderItem(OrderItem model)
          {
               var existingItem = _context.OrderItems
                    .Where(x => x.Id == model.Id)
                    .Include(x => x.JobDescription)
                    .Include(x => x.Remuneration)
                    .AsNoTracking().FirstOrDefault();

               if (existingItem == null) return;

               _context.Entry(existingItem).CurrentValues.SetValues(model);

               var jd = model.JobDescription;
               if (jd == null)
               {
                    jd = new JobDescription();
                    var newJd = new JobDescription(jd.JobDescInBrief ?? "Not Available",
                         jd.QualificationDesired ?? "Not Available",
                         jd.ExpDesiredMin, jd.ExpDesiredMax, jd.MinAge, jd.MaxAge);
                    existingItem.JobDescription = newJd;
                    _context.Entry(newJd).State = EntityState.Added;
               }
               else
               {
                    _context.Entry(existingItem.JobDescription).CurrentValues.SetValues(jd);
               }

               var rem = model.Remuneration;
               if (rem == null)
               {
                    rem = new Remuneration();
                    var newRem = new Remuneration(rem.SalaryCurrency ?? "???", rem.SalaryMin, rem.SalaryMax,
                         rem.ContractPeriodInMonths, rem.HousingProvidedFree, rem.HousingAllowance,
                         rem.FoodProvidedFree, rem.FoodAllowance, rem.TransportProvidedFree,
                         rem.TransportAllowance, rem.OtherAllowance, rem.LeavePerYearInDays,
                         rem.LeaveAirfareEntitlementAfterMonths);
                    existingItem.Remuneration = newRem;
                    _context.Entry(newRem).State = EntityState.Added;
               }
               else
               {
                    _context.Entry(existingItem.Remuneration).CurrentValues.SetValues(rem);
               }

               _context.Entry(existingItem).State = EntityState.Modified;
          }

          public bool EditOrderItemWithNavigationObjects(OrderItem modelItem)
          {
               throw new NotImplementedException();
          }

          public async Task<bool> DeleteOrderItem(OrderItem orderItem)
          {
               var spec = new CVRefSpecs(new CVRefSpecParams { OrderItemId = orderItem.Id });
               var cvref = await _unitOfWork.Repository<CVRef>().GetEntityWithSpec(spec);
               if (cvref != null) return false;

               _unitOfWork.Repository<OrderItem>().Delete(orderItem);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<JobDescription>> GetJobDescriptionsByOrderIdAsync(int orderId)
          {
               return await _context.JobDescriptions.Where(x => x.OrderId == orderId).ToListAsync();
          }

          public async Task<JDDto> GetOrAddJobDescription(int Id)
          {
               var item = await _context.OrderItems.Where(x => x.Id == Id)
                    .Include(x => x.Category)
                    .Include(x => x.JobDescription)
                    .FirstOrDefaultAsync();
               var or = await _context.Orders.Where(x => x.Id == item.OrderId)
                    .Include(x => x.Customer).Select(x => new {OrderNo=x.OrderNo, OrderDate= x.OrderDate, CustomerName= x.Customer.CustomerName})
                    .FirstOrDefaultAsync();
               if (or==null) return null;
               
               if(string.IsNullOrEmpty(item.CategoryName)) {
                    item.CategoryName = item.Category.Name;
               }
               if (item.JobDescription==null) {
                    var jd = new JobDescription{
                         OrderItemId = Id, OrderId = item.OrderId, OrderNo = or.OrderNo, JobDescInBrief="", };
                    _unitOfWork.Repository<JobDescription>().Add(jd);
                    if (await _unitOfWork.Complete() == 0) return null;
                    item.JobDescription = jd;
               }

               var jddto = new JDDto{
                    Id = item.JobDescription.Id, OrderItemId = Id, OrderId = item.OrderId, OrderNo = or.OrderNo, 
                    OrderDate = or.OrderDate, CustomerName = or.CustomerName, CategoryName = "",
                    JobDescInBrief = item.JobDescription.JobDescInBrief, QualificationDesired = item.JobDescription.QualificationDesired,
                    ExpDesiredMin = item.JobDescription.ExpDesiredMin, ExpDesiredMax = item.JobDescription.ExpDesiredMax, 
                    MinAge = item.JobDescription.MinAge, MaxAge = item.JobDescription.MaxAge
               };

               return jddto;
          }

          public void AddJobDescription(JobDescription jobDescription)
          {
               throw new NotImplementedException();
          }

          public async Task<bool> EditJobDescription(JDDto dto)
          {
               var jd = _mapper.Map<JobDescription>(dto);

               _unitOfWork.Repository<JobDescription>().Update(jd);

               return await _unitOfWork.Complete() > 0;
          }

          public void DeleteJobDescription(JobDescription jobDescription)
          {
               throw new NotImplementedException();
          }

          public async Task<IReadOnlyList<Remuneration>> GetRemunerationsByOrderIdAsync(int Id)
          {
               var spec = new RemunerationSpecs(new RemunerationSpecParams { OrderId = Id });
               var remun = await _unitOfWork.Repository<Remuneration>().ListAsync(spec);
               return remun;
          }

          public async Task<RemunerationFullDto> GetOrAddRemunerationByOrderItemAsync(int OrderItemId)
          {
               var item = await _context.OrderItems.Where(x => x.Id == OrderItemId)
                    .Include(x => x.Category)
                    .Include(x => x.Remuneration)
                    .FirstOrDefaultAsync();
               var or = await _context.Orders.Where(x => x.Id == item.OrderId)
                    .Include(x => x.Customer).Select(x => new {OrderNo=x.OrderNo, OrderDate= x.OrderDate, CustomerName= x.Customer.CustomerName})
                    .FirstOrDefaultAsync();
               if (or==null) return null;
               
               if(string.IsNullOrEmpty(item.CategoryName)) {
                    item.CategoryName = item.Category.Name;
               }

               if (item.Remuneration==null) {
                    var remun = new Remuneration{
                         OrderItemId = OrderItemId, OrderId = item.OrderId, OrderNo = or.OrderNo, 
                         WorkHours = 8, LeaveAirfareEntitlementAfterMonths=24, LeavePerYearInDays=21,
                         ContractPeriodInMonths=24,
                    };
                    _unitOfWork.Repository<Remuneration>().Add(remun);
                    if (await _unitOfWork.Complete() == 0) return null;
                    item.Remuneration = remun;
               }

               var remunDto = new RemunerationFullDto{
                    OrderDate = or.OrderDate, CustomerName = or.CustomerName, CategoryName = item.Category.Name,

                    Id = item.Remuneration.Id, OrderItemId = OrderItemId, OrderId = item.OrderId, OrderNo = or.OrderNo, 
                    CategoryId = item.CategoryId, WorkHours = item.Remuneration.WorkHours, 
                    SalaryCurrency = item.Remuneration.SalaryCurrency,
                    SalaryMin = item.Remuneration.SalaryMin, SalaryMax = item.Remuneration.SalaryMax,
                    ContractPeriodInMonths = item.Remuneration.ContractPeriodInMonths,
                    HousingProvidedFree = item.Remuneration.HousingProvidedFree, 
                    HousingAllowance = item.Remuneration.HousingAllowance,
                    HousingNotProvided = item.Remuneration.HousingNotProvided,
                    FoodProvidedFree = item.Remuneration.FoodProvidedFree,
                    FoodAllowance = item.Remuneration.FoodAllowance,
                    FoodNotProvided = item.Remuneration.FoodNotProvided,
                    TransportProvidedFree = item.Remuneration.TransportProvidedFree,
                    TransportAllowance = item.Remuneration.TransportAllowance,
                    TransportNotProvided = item.Remuneration.TransportNotProvided,
                    OtherAllowance = item.Remuneration.OtherAllowance,
                    LeavePerYearInDays = item.Remuneration.LeavePerYearInDays,
                    LeaveAirfareEntitlementAfterMonths = item.Remuneration.LeaveAirfareEntitlementAfterMonths
               };

               return remunDto;
          }

          public async Task<Remuneration> AddRemuneration(Remuneration remuneration)
          {
               _unitOfWork.Repository<Remuneration>().Add(remuneration);
               if (await _unitOfWork.Complete() > 0)
               {
                    return await _unitOfWork.Repository<Remuneration>().GetEntityWithSpec(
                         new RemunerationSpecs(remuneration.OrderItemId));
               }
               else
               {
                    return null;
               }
          }

          public async Task<bool> EditRemuneration(RemunerationFullDto dto)
          {
               var remun = _mapper.Map<Remuneration>(dto);

               _unitOfWork.Repository<Remuneration>().Update(remun);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> DeleteRemuneration(Remuneration remuneration)
          {
               _unitOfWork.Repository<Remuneration>().Delete(remuneration);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> OrderForwardedToHRDept(int orderId)
          {
               var order = await _context.Orders.FindAsync(orderId);
               return ((DateTime)order.ForwardedToHRDeptOn).Year > 2000;
          }

          public async Task<ICollection<CustomerCity>> GetOrderCityNames()
     {
          var citynames = await (from o in _context.Orders
               join cust in _context.Customers on o.CustomerId equals cust.Id
               select cust.City).Distinct()
               .ToListAsync();

          var lsts = new List<CustomerCity>();
          foreach(var lst in citynames)
          {
               lsts.Add(new CustomerCity{CityName = lst});
          }
          return lsts;
     }

          public async Task<bool> UpdateDLForwardedDateToHR(int orderid, DateTime forwardedToHROn)
          {
               var order = await _context.Orders.FindAsync(orderid);
               if (order == null) return false;

               order.ForwardedToHRDeptOn = forwardedToHROn;

               _unitOfWork.Repository<Order>().Update(order);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsNotPaged()
          {
               var  qry = await (from i in _context.OrderItems
                    where(i.ReviewItemStatusId == EnumReviewItemStatus.Accepted
                         && (i.Status == (int) EnumOrderItemStatus.UnderProcess 
                         || i.Status == (int)EnumOrderItemStatus.NotStarted))
                    join o in _context.Orders on i.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ass in _context.OrderItemAssessments on i.Id equals ass.OrderItemId into itemAss 
                         from itemAssessments in itemAss.DefaultIfEmpty()
                    select new OrderItemBriefDto {
                         OrderItemId = i.Id, OrderId = o.Id, 
                         CustomerName = c.CustomerName, OrderDate = o.OrderDate,
                         CategoryId = i.CategoryId, CategoryName = cat.Name,
                         Quantity = i.Quantity, Status = (int)i.Status,
                         CategoryRef = o.OrderNo + "-" + i.SrNo,
                         CategoryRefAndName = cat.Name + "(" + o.OrderNo + "-" + i.SrNo + ")",
                         RequireInternalReview = i.RequireInternalReview,
                         AssessmentQDesigned = (itemAssessments.OrderItemId > 0)
                    }).ToListAsync();

               /*var qry2 = await _context.Orders.Where(x => x.Status == EnumOrderStatus.ReviewedAndAccepted)
                         .Include(x => x.Customer)
                         .Include(x => x.OrderItems.Where(y => y.ReviewItemStatusId == EnumReviewItemStatus.Accepted 
                                   && (y.Status == EnumOrderItemStatus.UnderProcess || y.Status == EnumOrderItemStatus.NotStarted)))
                              .ThenInclude(x => x.Category)
                         .Select new OrderItemBriefDto(x => {
                              OrderItemId =  x.OrderItems i.Id, OrderId = o.Id, 
                              CustomerName = c.CustomerName, OrderDate = o.OrderDate,
                              CategoryId = i.CategoryId, CategoryName = cat.Name,
                              Quantity = i.Quantity, Status = (int)i.Status

                         })
                    )
               */
               
               return qry;
          }

          public async Task<ICollection<OrderItemBriefDto>> GetOrderItemsBriefDtoByOrderId(int OrderId)
          {
               var spec = new OrderItemSpecs(OrderId);
               var items = await _orderItemRepo.ListAsync(spec);
               if (items==null) return null;
               return _mapper.Map<ICollection<OrderItem>, ICollection<OrderItemBriefDto>>((ICollection<OrderItem>)items);
          }

          public async Task<OrderBriefDtoR> GetOrderBrief(int OrderId)
          {
               
               var qry = await (from o in _context.Orders where o.Id == OrderId
                    join c in _context.Customers on o.CustomerId equals c.Id 
                    select new {Id = o.Id, OrderNo = o.OrderNo, OrderDate = o.OrderDate, CustomerName = c.CustomerName}
                    ).FirstOrDefaultAsync();
               
               if (qry == null) return null;

               var items = await (from i in _context.OrderItems where i.OrderId == OrderId
                    join c in _context.Categories on i.CategoryId equals c.Id
                    select new {OrderId = OrderId, OrderItemId = i.Id, CategoryRef = qry.OrderNo + "-" + i.SrNo, CategoryName = c.Name, Quantity = i.Quantity}
                    ).ToListAsync();
               var dtoitems = new List<OrderItem_Dto>();
               foreach(var item in items) {
                    dtoitems.Add(new OrderItem_Dto{OrderId = OrderId, OrderItemId = item.OrderItemId, CategoryRef = item.CategoryRef, 
                         CategoryName = item.CategoryName, Quantity = item.Quantity});
               }
               var dto = new OrderBriefDtoR{OrderNo = qry.OrderNo, OrderDate = qry.OrderDate, CustomerName = qry.CustomerName, Items = dtoitems};

               return dto;
          }

     }

}