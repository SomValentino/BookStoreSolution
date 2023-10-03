using MongoDB.Driver;
using Order.API.Models;

namespace Order.API.Data.Repository.Interfaces;

public interface IOrderRepository {

    Task<Order.API.Models.Order> GetOrderById (string orderId);
    Task<IEnumerable<Order.API.Models.Order>> GetOrdersByUsername (string username);
    Task<IEnumerable<Order.API.Models.Order>> GetOrdersByQuery(FilterDefinition<Models.Order> filter);
        Task CreateOrder (Order.API.Models.Order order);
    Task<bool> UpdateOrder (Order.API.Models.Order order);
    Task<bool> DeleteOrder (Order.API.Models.Order order);
}