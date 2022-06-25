using System.Collections.Generic;
using System.Threading.Tasks;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IStatsService
    {
         Task<ICollection<OpeningsDto>> GetCurrentOpenings (StatsParams param);
         //Task<OrderItemTransDto> GetOrderItemTransactions (StatsTransParams param);
         Task<Pagination<EmployeePerformanceDto>> GetEmployeePerformance (EmployeePerfParams empPerf);
    }
}