using core.Entities.Orders;
using core.Entities.Users;

namespace core.ParamsAndDtos
{
    public class OrderMessageParamDto
    {
        public Order Order { get; set; }
        public bool DirectlySendMessage { get; set; }
    }
}