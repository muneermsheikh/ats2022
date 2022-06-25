using System.Threading.Tasks;
using core.Entities.Orders;

namespace core.Interfaces
{
    public interface IPaymentService
    {
        //Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}