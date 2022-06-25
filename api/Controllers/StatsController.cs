using System.Collections.Generic;
using System.Threading.Tasks;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class StatsController : BaseApiController
     {
          private readonly IStatsService _statsService;
          public StatsController(IStatsService statsService)
          {
               _statsService = statsService;
          }

          //openings

          [HttpGet("openings")]
          public async Task<ActionResult<ICollection<OpeningsDto>>> GetCurrentOpenings([FromQuery] StatsParams param)
          {
               var t = await _statsService.GetCurrentOpenings(param);
               if (t == null) return BadRequest("No opening found");

               //Response.AddPaginationHeader(t.CurrentPage, t.PageSize, t.TotalCount, t.TotalPages);
               return Ok(t);

          }

     /*
          [HttpGet("ordertrans/{orderid}")]
          public async Task<ActionResult<OrderItemTransDto>> GetOrderItemTransactions(int orderid)
          {
               var lst = new List<int>();
               lst.Add(orderid);
               var param = new StatsTransParams();
               param.OrderIds = lst;

               var op = await _statsService.GetOrderItemTransactions(param);

               if (op == null) return BadRequest("Bad Request - error");
               //Response.AddPaginationHeader(op.CurrentPage, op.PageSize, op.TotalCount, op.TotalPages);
               return Ok(op);
          }
     */
     /*
          [HttpGet("itemtrans/{orderitemid}")]
          public async Task<ActionResult<OrderItemTransDto>> GetOrderItemsTransactions(int orderitemid)
          {
               var lst = new List<int>();
               lst.Add(orderitemid);
               var param = new StatsTransParams();
               param.OrderItemIds = lst;

               var op = await _statsService.GetOrderItemTransactions(param);

               if (op == null) return BadRequest("Bad Request - error");
               //Response.AddPaginationHeader(op.CurrentPage, op.PageSize, op.TotalCount, op.TotalPages);
               return Ok(op);
          }
     */
     /*
          [HttpGet("aitemstrans")]
          public async Task<ActionResult<OrderItemTransDto>> GetOrderItemTransactions(StatsTransParams param)
          {
               var op = await _statsService.GetOrderItemTransactions(param);

               if (op == null) return BadRequest("Bad Request - error");
               //Response.AddPaginationHeader(op.CurrentPage, op.PageSize, op.TotalCount, op.TotalPages);
               return Ok(op);
          }
     */
     /*
          [HttpGet("itemstrans")]
          public async Task<ActionResult<ICollection<OrderItemTransDto>>> GetOrderItemsTransactions([FromQuery] StatsTransParams param)
          {
               var op = await _statsService.GetOrderItemTransactions(param);

               if (op == null) return BadRequest("Bad Request - error");
               //Response.AddPaginationHeader(op.CurrentPage, op.PageSize, op.TotalCount, op.TotalPages);
               return Ok(op);
          }
     */
     }
}