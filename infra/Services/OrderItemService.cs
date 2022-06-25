using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using core.Entities.Orders;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class OrderItemService : IOrderItemService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly IMapper _mapper;
          public OrderItemService(ATSContext context, IUnitOfWork unitOfWork, IMapper mapper)
          {
               _mapper = mapper;
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<OrderItemBriefDto> GetOrderItemRBriefDtoFromOrderItemId(int OrderItemId)
          {
               var orderItem = await _context.OrderItems.FindAsync(OrderItemId);
               if (orderItem == null) return null;
               
               return await ConvertOrderItemToBriefDto(orderItem);
          }

          public async Task<OrderItemBriefDto> GetOrderItemBriefDtoFromOrderItem(OrderItem orderItem)
          {
               return await ConvertOrderItemToBriefDto(orderItem);
               
          }

          private async Task<OrderItemBriefDto> ConvertOrderItemToBriefDto(OrderItem orderItem)
          {
                if (orderItem == null) return null;

               var dto = _mapper.Map<OrderItem, OrderItemBriefDto>(orderItem);
            
               var details = await (from i in _context.OrderItems where i.Id == orderItem.OrderId
                    join o in _context.Orders on i.OrderId equals o.Id
                    join cust in _context.Customers on o.CustomerId equals cust.Id
                    join c in _context.Categories on i.CategoryId equals c.Id
                    select new {OrderNo=i.OrderNo, CategoryName=c.Name, CustomerName = cust.CustomerName, OrderDate = o.OrderDate})
                    .FirstOrDefaultAsync();
                dto.CategoryName = details.CategoryName;
                dto.CategoryRef = details.OrderNo + "-"+ orderItem.SrNo;
                dto.CategoryRefAndName = dto.CategoryName + "(" + dto.CategoryRef + ")";
                dto.CustomerName = details.CustomerName;
                dto.OrderDate = details.OrderDate;
                dto.Status = orderItem.Status;
                dto.OrderItemId = orderItem.Id;
                dto.OrderNo=details.OrderNo;

                return dto;
          }
     }
}