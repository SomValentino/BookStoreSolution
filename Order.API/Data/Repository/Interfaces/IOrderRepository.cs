using Order.API.Models;

namespace Order.API.Data.Repository.Interfaces;

public interface IOrderRepository {

    Task<Order.API.Models.Order> GetOrderById(Guid orderId);
    Task<IEnumerable<Order.API.Models.Order>> GetOrdersByUsername (string username);
    Task CreateOrder (Order.API.Models.Order order);
    Task<bool> UpdateOrder (Order.API.Models.Order order);
    Task<bool> DeleteOrder (Order.API.Models.Order order);
}